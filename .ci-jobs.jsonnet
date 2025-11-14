local module = std.extVar('MODULE');

local testJob(m) = {
  stage: 'build',
  allow_failure: true,
  image: 'mcr.microsoft.com/dotnet/sdk:8.0-alpine',
  script: [
    'export FOLDER="Modules/%s/Tests"' % m,
    '[ ! -d "${FOLDER}" ] && exit 0',
    'dotnet test -c Release ${FOLDER}',
  ],
};

local buildJob(m) = {
  stage: 'build',
  when: 'manual',
  needs: ['%s/test' % std.asciiLower(m)],
  image: '${CI_DEPENDENCY_PROXY_GROUP_IMAGE_PREFIX}/docker:stable-dind',
  script: [
    'export IMAGE_NAME="$HARBOR_HOST/$HARBOR_PROJECT/$(echo ${CI_PROJECT_PATH} | awk -F / \'{print $2"/"$3}\')/%s"' % std.asciiLower(m),
    'docker build --build-arg MODULENAME=%s -t "${IMAGE_NAME}:${CI_COMMIT_SHORT_SHA}" -f ./Dockerfile .' % m,
    'docker tag "${IMAGE_NAME}:${CI_COMMIT_SHORT_SHA}" "${IMAGE_NAME}:latest"',
    'docker login -u "$HARBOR_USERNAME" -p "$HARBOR_PASSWORD" "${HARBOR_HOST}"',
    'docker push "${IMAGE_NAME}:${CI_COMMIT_SHORT_SHA}"',
    'docker push "${IMAGE_NAME}:latest"',
  ],
};

local deployJob(m) = {
  stage: 'deploy',
  tags: ['my-mini-stg'],
  needs: ['%s/build' % std.asciiLower(m)],
  services: [
    {
      name: '${CI_DEPENDENCY_PROXY_GROUP_IMAGE_PREFIX}/docker:stable-dind',
      alias: 'docker',
    },
  ],
  before_script: [
    'docker login -u $CI_DEPENDENCY_PROXY_USER -p $CI_DEPENDENCY_PROXY_PASSWORD $CI_DEPENDENCY_PROXY_SERVER'
  ],
  image: '${CI_DEPENDENCY_PROXY_GROUP_IMAGE_PREFIX}/docker:stable-dind',
  script: [
    'export MODULENAME="%s"' % std.asciiLower(m),
    'export IMAGE_NAME="$HARBOR_HOST/$HARBOR_PROJECT/$(echo ${CI_PROJECT_PATH} | awk -F / \'{print $2"/"$3}\')/${MODULENAME}"',
    'mkdir ~/.ssh',
    'ssh-keyscan -p "${STAGE_PORT}" "${STAGE_HOST}" > ~/.ssh/known_hosts',
    'echo -e "-----BEGIN OPENSSH PRIVATE KEY-----\n${STAGE_KEY}\n-----END OPENSSH PRIVATE KEY-----" > ~/.ssh/id_ed25519',
    'chmod -R 600 ~/.ssh',
    'docker login -u "$HARBOR_USERNAME" -p "$HARBOR_PASSWORD" "${HARBOR_HOST}"',
    'docker context create ${STAGE_STACK} --docker "host=ssh://${STAGE_USER}@${STAGE_HOST}:${STAGE_PORT}"',
    'docker --context ${STAGE_STACK} service update ${STAGE_STACK}_${MODULENAME} --force --with-registry-auth --image ${IMAGE_NAME}:${CI_COMMIT_SHORT_SHA}',
  ],
};

{
  include: 'base.yaml',
} + {
  ['%s/test' % std.asciiLower(module)]: testJob(module),
} + {
  ['%s/build' % std.asciiLower(module)]: buildJob(module),
} + {
  ['%s/deploy' % std.asciiLower(module)]: deployJob(module),
}

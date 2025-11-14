local modules = std.split(std.extVar('MODULES'), ' ');

local pipeline(module) = {
  when: 'always',
  stage: 'setup',
  needs: ['generate-jobs'],
  trigger: {
    include: [
      {
        artifact: 'generated-%s-jobs.yml' % [std.asciiLower(module)],
        job: 'generate-jobs',
      },
    ],
    strategy: 'depend',
  },
};

{
  include: 'base.yaml',

  'generate-jobs': {
    extends: '.base',
    stage: 'setup',
    when: 'always',
    script: [
      'jsonnet --version',
      ['jsonnet --ext-str "MODULE=%s" .ci-jobs.jsonnet > generated-%s-jobs.yml' % [m, std.asciiLower(m)] for m in modules],
    ],
    artifacts: {
      paths: ['generated-*-jobs.yml'],
    },
  },
} + {
  ['%s' % module]: pipeline(module)
  for module in modules
}

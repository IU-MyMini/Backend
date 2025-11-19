# Deployment to staging and production

Automate deployment to staging and production environment is done in 
- [.gitlab-ci.yml](../.gitlab-ci.yml) - define pipeline generation for code building, testing, deploying to staging in any branch (on developers demand) and production environment deploy on release creation (`release-tag-build`, `release-tag-deploy` jobs) 
- [.gitlab-ci.jsonnet](../.gitlab-ci.jsonnet) - define pipeline structure for each module (in this case `Grading` only)
- [.ci-jobs.jsonnet](../.ci-jobs.jsonnet) - define build, test and deploy to staging jobs for specific module

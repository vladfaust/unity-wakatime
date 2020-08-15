#!/bin/bash

set -e

eval "$(ssh-agent -s)"
openssl aes-256-cbc -K $encrypted_b77369d471a1_key -iv $encrypted_b77369d471a1_iv -in CI/github_deploy_key.enc -out github_deploy_key -d
chmod 600 github_deploy_key
ssh-add github_deploy_key

TARGET_BRANCH=package
FOLDER_TO_EXPORT=Assets/com.vladfaust.unitywakatime

# Put the ubiquitous meta files back
rsync -av CI/Meta/ FOLDER_TO_EXPORT

REMOTE=$(git config --get remote.origin.url)
COMMIT=$(git log -1 --pretty=%B)

git archive -o archive.tar HEAD:$FOLDER_TO_EXPORT
ARCHIVE_PATH=$(pwd)

mkdir ../$TARGET_BRANCH
cd ../$TARGET_BRANCH

if [ "$(git ls-remote $REMOTE $TARGET_BRANCH | wc -l)" != 1 ]; then
    git clone --depth=1 $REMOTE
    cd *
    git checkout -b $TARGET_BRANCH
else
    git clone --branch=$TARGET_BRANCH $REMOTE
    cd *
fi

shopt -s extglob
rm -rf -- !(.git|.|..)

mv $ARCHIVE_PATH/archive.tar archive.tar

tar -xf archive.tar --overwrite
rm archive.tar

git add -A

git config --global user.email "travis@travis-ci.org"
git config --global user.name "Travis CI"

git commit -m "$COMMIT" || true

git push git@github.com:${TRAVIS_REPO_SLUG}.git $TARGET_BRANCH

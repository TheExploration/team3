# Contributing

Either Use Command Line Steps or Github Desktop

## Github Desktop

...someone add


## Command Line/Command Prompt/Cmd

Everyone should have team3 git repository cloned already.

1. Open the project folder 'team3' right click the blank area in the folder and press 'Open in Terminal'

2. To download the changes from the github repo (run everytime, before you work in Unity) Run: `git pull --all` > downloads all the changes from the github and updates everything locally. 

3. Then switch to the networking branch using `git checkout BRANCH_NAME` BEFORE YOU MAKE CHANGES IN UNITY

4. Then you can code and make your changes!

5. To push your updated version to the Github, run `git add .`, then `git commit -m update`, follow the instructions it gives you, then run `git push -u` and it should ask you to sign in to github, then the code will be uploaded online for everyone. 

##### IF ANYONE RUNS INTO CONFLICTS WITH MERGING:
To reset your local files to match the online Github project, run 
`git fetch`
then
`git reset --hard`
then
`git clean -x -d -f`

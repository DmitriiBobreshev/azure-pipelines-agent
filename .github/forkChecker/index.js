const rm = require('typed-rest-client/RestClient');
const core = require('@actions/core');
const github = require('@actions/github');

async function main() {
    try {
        const pullRequestNumber = github.context.issue.number;
        console.log(`Running for PR: ${pullRequestNumber}\n`);
        let rest = new rm.RestClient('forkChecker');
        console.log('Getting PR info info\n');
        let res = await rest.get(`https://api.github.com/repos/DmitriiBobreshev/azure-pipelines-agent/pulls/${pullRequestNumber}/labels`);
        const isFork = res.result?.head?.repo?.fork;
        console.log(`IsFork: ${isFork}`);
        if (isFork) {
            throw `The PRs from the forked repository don't allowed due security reasons. Please, create a new PR from the original repository.`;
        }
    } catch (err) {
        core.setFailed(err);
    }

}

main();

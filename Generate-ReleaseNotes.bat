rem See https://github.com/StefH/GitHubReleaseNotes for more information.

SET version=3.1.0

GitHubReleaseNotes --output ReleaseNotes.md --skip-empty-releases --exclude-labels question invalid doc --version %version%
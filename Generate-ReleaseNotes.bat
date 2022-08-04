rem See https://github.com/StefH/GitHubReleaseNotes for more information.

SET version=3.0.23

GitHubReleaseNotes --output ReleaseNotes.md --skip-empty-releases --exclude-labels question invalid doc --version %version%
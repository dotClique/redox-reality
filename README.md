# RedoXReality
This is a Virtual Reality project for the course TDT16 - Extended Reality at
NTNU. The app allows simulated pH testing of liquids in VR - simply grab the
microphone, hold the right trigger and say the name of your liquid. The liquid
will appear, with ChatGPT having chosen its color and pH. You can then dip the
paper swab in the liquid and compare the resulting color to the reference
chart. If you speak your guess into the microphone, you can see the 'correct'
(according to ChatGPT) answer, and ask for another liquid. The app is tested
on Meta Quest 2, 3 and Pro, and would probably work on other OpenXR devices
with a different speech-to-text package.

## Building
This project uses Git LFS with a self-hosted LFS server. To install Git LFS,
run `git lfs install`. After that, the repository can be cloned as usual. You
may be prompted for credentials for the LFS server, in which case you can leave
both username and password blank for read-access. To run the project with all
features, you will need an API key from OpenAI, which must be placed in
`Assets/Secrets.cs` (copied from `Assets/SecretsTemplate.cs`, remember to
rename the class). You will also need to generate a token for Wit, which can be
done at [wit.ai](https://wit.ai/). It then needs to be added to the Wit
Configuration, which is located in `Assets` and named `VoiceNLP`.

## Developing
If you would like to fork the repository, you can self-host your own LFS server
using for example [rudolfs](https://github.com/jasonwhite/rudolfs). In that
case, change the URL in `.lfsconfig` to your own server.

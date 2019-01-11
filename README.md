# WireTap

WireTap is a .NET 4.0 project to consolidate several functions used to interact with a user's hardware, including:
	
	- Screenshots (Display + WebCam Imaging)
	- Audio (Both line-in and line-out)
	- Keylogging
	- Activate voice recording when the user says a keyword phrase.

For each function, WireTap will write the resultant file into the temp directory with the appropriate suffix.

## Usage

```
WireTap.exe [arguments]

Arguments can be one (and only one) of the following:
    record_mic [10s]     - Record audio from the attached microphone (line-in).
                          Time suffix can be s/m/h.

    record_sys [10s]     - Record audio from the system speakers (line-out).
                          Time suffix can be s/m/h.

    record_audio [10s]   - Record audio from both the microphone and the speakers.
                           Time suffix can be s/m/h.

    capture_screen       - Screenshot the current user's screen.

    capture_webcam       - Capture images from the user's attached webcam (if it exists).

    keylogger            - Begin logging keystrokes to a file.

    listen_for_passwords - Listens for words 'username', 'password', 'login' and 'credential', and when heard,
                           starts an audio recording for two minutes.


Examples:
    Record all audio for 30 seconds:
        WireTap.exe record_audio 30s

    Start the keylogger:
        WireTap.exe keylogger

    Start keyword listener with a custom set of words:
        WireTap.exe listen_for_passwords oil,password,pin
```

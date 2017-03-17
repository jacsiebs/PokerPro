# PokerPro
Instructuons to view and run the code:

Navigate to the “develop” branch and download the zip.
To check out the code, navigate to the assets/scripts folder which contains all frontend code. Once you've checked it out there are multiple ways to run the code:

-Open the folder in the Unity editor. To run the code in editor, simply open the login scene and click the play button. An android emulator can also be used. If in editor, pressing the login button will simulate a facebook login, as where on a device the login will function as normal. Simply click “Find user access token” and copy and paste the user token into the entry field and send success. Navigate to the game view by selecting cash game in the main menu. The server may not respond due to technical errors and therefore if the loading scene does not progress, simply click the button to continue. In the game view there are debug buttons (can be used in any order) to simulate various parts of a game. Cards values may not be accurate as the real values would be provided by the server. Nevertheless the frontend code to interact with the server has been written although the server is often too inconsistent (often responses do not come back) for the full process to complete without error. 

-Put the pokerPro_test apk on an android device and run it.
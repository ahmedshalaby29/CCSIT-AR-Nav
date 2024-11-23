/**
 * Import function triggers from their respective submodules:
 *
 * import {onCall} from "firebase-functions/v2/https";
 * import {onDocumentWritten} from "firebase-functions/v2/firestore";
 *
 * See a full list of supported triggers at https://firebase.google.com/docs/functions
 */

// Start writing functions
// https://firebase.google.com/docs/functions/typescript

// export const helloWorld = onRequest((request, response) => {
//   logger.info("Hello logs!", {structuredData: true});
//   response.send("Hello from Firebase!");
// });
import functions = require("firebase-functions");
import admin = require("firebase-admin");
admin.initializeApp();

exports.notifyNewMessage = functions.firestore
  .document("messages/{messageId}")
  .onCreate(async (snapshot, context) => {
    const messageData = snapshot.data();

    if (!messageData) {
      console.error("No message data found!");
      return;
    }

    const sender = messageData.sender || "Unknown Sender";
    const messageText = messageData.messageText || "No message text";

    // Retrieve the FCM tokens of all users
    const tokensSnapshot = await admin.firestore().collection("users").get();

    const tokens = tokensSnapshot.docs
      .map((doc) => doc.data().fcmToken)
      .filter((token) => token);

    if (tokens.length === 0) {
      console.log("No tokens available to send notifications.");
      return;
    }
    const notificationPayload = {
      notification: {
        title: `New message from ${sender}`,
        body: messageText,
      },
      tokens, // Optional: Define custom click behavior
      data: { clickAction: "FLUTTER_NOTIFICATION_CLICK" },
    };
    // Send the notification to all tokens
    const response = await admin
      .messaging()
      .sendEachForMulticast(notificationPayload);

    console.log("Notifications sent successfully:", response);
  });

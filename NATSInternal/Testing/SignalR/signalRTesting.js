const signalR = require("@microsoft/signalr");
const host = "http://localhost:5000/api"
let accessToken;

// Get access token.
fetch(`${host}/authentication/getAccessToken`, {
    headers: {
        "Content-Type": "application/json"
    },
    method: "POST",
    body: JSON.stringify({
        "userName": "ngokhanhhuyy",
        "password": "Huyy47b1"
    })
}).then(response => response.json().then(data => {
    // Replace with your actual SignalR hub URL and access token
    const hubUrl = `${host}/notificationHub`;
    accessToken = data.accessToken;
    console.log(accessToken);
    
    // Create the connection to the SignalR hub
    const connection = new signalR.HubConnectionBuilder()
        .withUrl(hubUrl, {
            accessTokenFactory: () => accessToken
        })
        .build();
    
    // Start the connection
    connection.start()
        .then(() => {
            console.log("Connected to SignalR hub");
        })
        .catch(err => {
            console.error("Error connecting to SignalR hub", err);
        });
    
    // Handle incoming messages
    connection.on("ReceiveMessage", (message) => {
        console.log("Message received:", message);
    });
    
    // Example of sending a message to the hub
    connection.onclose(() => {
        console.log("Connection closed");
    });
    
    connection.on("NotificationDistributed", message => {
        console.log("NotificationDistributed");
        console.log(message);
    });
    
    // Keep the process alive
    process.stdin.resume();
}));


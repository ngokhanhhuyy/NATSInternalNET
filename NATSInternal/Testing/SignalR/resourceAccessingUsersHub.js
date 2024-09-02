import { HubConnectionBuilder } from "@microsoft/signalr";

const host = "http://localhost:5000/api"
let connection;
let accessToken;

// Get access token.
fetch(`${host}/authentication/getAccessToken?includeAccessToken=False`, {
    headers: {
        "Content-Type": "application/json"
    },
    method: "POST",
    body: JSON.stringify(process.argv[2] === "ngokhanhhuyy" ? {
        "userName": "ngokhanhhuyy",
        "password": "Huyy47b1"
    } : {
        "userName": "anhtaingo",
        "password": "tai123"
    })
}).then(response => response.json().then(data => {
    accessToken = data.accessToken;

    connection = new HubConnectionBuilder()
        .withUrl(`${host}/resourceAccessHub`, {
            accessTokenFactory: () => accessToken
        }).build();

    // Start the connection
    connection.start()
        .then(() => {
            console.log("Connected to ResourceAccessingUsers hub");
            connection.on(
                "ResourceAccessStarted",
                (resourceName, resourcePrimaryId, resourceSecondaryId, userResponseDto) => {
                    const json = {
                        resourceName,
                        resourcePrimaryId,
                        resourceSecondaryId,
                        userResponseDto
                    }
                    console.log(json);
                });
        
            connection.on(
                "ResourceAccessFinished",
                (resourceName, resourcePrimaryId, resourceSecondaryId, userResponseDto) => {
                    const json = {
                        resourceName,
                        resourcePrimaryId,
                        resourceSecondaryId,
                        userResponseDto: JSON.stringify(userResponseDto, null, 2)
                    }
                    console.log(json);
                });

            connection.send("AccessResource", "User", 1, null);
        }).catch(err => {
            console.error("Error connecting to ResourceAccessingUsers hub", err);
        });
}));
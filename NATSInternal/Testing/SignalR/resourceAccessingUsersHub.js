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
        .withUrl(`${host}/hub`, {
            accessTokenFactory: () => accessToken
        }).build();

    // Start the connection
    connection.start()
        .then(() => {
            console.log("Connected to ResourceAccessingUsers hub");
            connection.on(
                "Other.ResourceAccessStarted",
                (resource, userResponseDto) => {
                    const json = {
                        resource,
                        user: userResponseDto.userName,
                        type: "Other.ResourceAccessStarted"
                    };
                    console.log(JSON.stringify(json, null, 2));
                });
            connection.on(
                "Self.ResourceAccessStarted",
                (resource, listResponseDto) => {
                    const json = {
                        resource,
                        user: listResponseDto.results.map(u => u.userName),
                        type: "Self.ResourceAccessStarted"
                    };
                    console.log(JSON.stringify(json, null, 2));
                });
        
            connection.on(
                "Other.ResourceAccessFinished",
                (resource, userId) => {
                    const json = {
                        resource,
                        userId,
                        type: "Other.ResourceAccessFinished"
                    };
                    console.log(JSON.stringify(json, null, 2));
                });

            connection.send("StartResourceAccess", {
                type: "User",
                primaryId: 1,
                secondaryId: null,
                mode: 1
            }).catch(err => console.log(err));
        }).catch(err => {
            console.error("Error connecting to ResourceAccessingUsers hub", err);
        });
}));
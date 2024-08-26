const accessToken = client.global.get("accessToken");
if (!accessToken) {
    const authRequest = new HttpRequest();
    authRequest.setHeader("Content-Type", "application/json");
    authRequest.setBodyJson({
        "userName": client.global.get("userName"),
        "password": client.global.get("password")
    });
    const authResponse = await client.send(authRequest);
    client.global.set("accessToken", authResponse.json()["accessToken"]);
}
client.
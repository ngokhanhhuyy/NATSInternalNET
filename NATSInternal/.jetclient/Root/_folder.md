```toml
name = 'Root'
icon = 'SOURCE_ROOT'
sortWeight = 3000000
id = '89d21e11-ab65-4938-b82b-506f3eb06a3c'

[[headers]]
key = 'Authorization'
value = 'Bearer {{accessToken}}'
disabled = true

[auth.bearer]
token = '{{accessToken}}'
```

#### Variables

```json5
{
  accessToken: ""
}
```

#### Pre-request Script

```js
const accessToken = jc.globals.get("accessToken");
jc.request.setHeader("Authorization", `Bearer ${accessToken}`);
```

#### Post-response Script

```js
if (jc.response.code === 401) {
    console.log("Unauthorized, sending authentication request...");
    const authRequest = new HttpRequest();
    authRequest.url = jc.globals.get("host") + "/authentication/getAccessToken?includeExchangeToken=False";
    authRequest.method = "POST";
    authRequest.setHeader("Content-Type", "application/json");
    authRequest.setBodyJson({
        "userName": "ngokhanhhuyy",
        "password": "Huyy47b1"
    });
    jc.sendRequest(authRequest).then(authResponse => {
        if (authResponse.code === 200) {
            jc.globals.set("accessToken", authResponse.json()["accessToken"]);
        }
        console.log("Got access token successfully, please try to request again!");
    });
}
```

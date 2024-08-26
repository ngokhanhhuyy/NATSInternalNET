```toml
name = 'Root'
sortWeight = 3000000
id = '89d21e11-ab65-4938-b82b-506f3eb06a3c'

[auth.bearer]
token = '{{accessToken}}'
```

#### Variables

```json5
{
  accessToken: ""
}
```

#### Post-response Script

```js
if (jc.response.code !== 200 || jc.response.code !== 201) {
    const authRequest = new HttpRequest();
    authRequest.setHeader("Content-Type", "application/json");
    authRequest.setBodyJson({
        "userName": "ngokhanhhuyy",
        "password": "Huyy47b1"
    });
    jc.sendRequest(authRequest).then(authResponse => {
        if (authResponse.code === 200) {
            jc.globals.set("accessToken", authResponse.json()["accessToken"]);
        }
    });
}
```

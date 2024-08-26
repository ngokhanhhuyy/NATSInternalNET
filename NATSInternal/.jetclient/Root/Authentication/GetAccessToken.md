```toml
name = 'GetAccessToken'
method = 'POST'
url = '{{host}}/authentication/getAccessToken'
sortWeight = 1000000
id = 'd0e7d179-8fa0-4a8c-ae29-3094b47de1e2'

[body]
type = 'JSON'
raw = '''
{
    "userName": "ngokhanhhuyy",
    "password": "Huyy47b1"
}'''
```

#### Post-response Script

```js
if (jc.response.code === 200) {
    const accessToken = jc.response.json()["accessToken"];
    console.log(accessToken);
    jc.globals.set("accessToken", accessToken);
}
```

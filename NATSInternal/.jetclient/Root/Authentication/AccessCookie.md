```toml
name = 'AccessCookie'
method = 'POST'
url = '{{host}}/authentication/getAccessCookie'
sortWeight = 2000000
id = '5fbd8d63-ff93-4c21-a594-69fcd363c41b'

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
    const accessToken = jc.response.headers;
    console.log(accessToken);
    // jc.environment["shared"].set("accessCookie", accessToken);
}
```

# Change Password Endpoint

## Overview

The Change Password endpoint (`POST /auth/change-password`) allows authenticated users to change their password by providing their current password and a new password.

## Endpoint Details

- **URL**: `/auth/change-password`
- **Method**: `POST`
- **Authentication**: Required (JWT Bearer token)
- **Content-Type**: `application/json`

## Request Body

```json
{
  "currentPassword": "string (8-100 characters)",
  "newPassword": "string (8-100 characters)",
  "confirmNewPassword": "string (must match newPassword)"
}
```

### Field Validation

| Field | Type | Required | Constraints |
|-------|------|----------|-------------|
| `currentPassword` | string | Yes | 8-100 characters |
| `newPassword` | string | Yes | 8-100 characters |
| `confirmNewPassword` | string | Yes | Must match `newPassword` |

## Success Response

**Code**: `200 OK`

```json
{
  "success": true,
  "message": "Password changed successfully"
}
```

## Error Responses

### Current Password Incorrect

**Code**: `400 Bad Request`

```json
{
  "success": false,
  "message": "Current password is incorrect"
}
```

### Invalid or Missing Authentication

**Code**: `401 Unauthorized`

```json
{
  "success": false,
  "message": "Invalid or missing user authentication"
}
```

### Validation Errors

**Code**: `400 Bad Request`

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "NewPassword": [
      "The field NewPassword must be a string with a minimum length of 8 and a maximum length of 100."
    ],
    "ConfirmNewPassword": [
      "New password and confirmation do not match"
    ]
  }
}
```

## Examples

### cURL

```bash
curl -X POST http://localhost:5000/auth/change-password \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -d '{
    "currentPassword": "youser123",
    "newPassword": "NewSecurePass123!",
    "confirmNewPassword": "NewSecurePass123!"
  }'
```

### JavaScript (Fetch API)

```javascript
const changePassword = async (currentPassword, newPassword) => {
  try {
    const response = await fetch('http://localhost:5000/auth/change-password', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${localStorage.getItem('token')}`
      },
      body: JSON.stringify({
        currentPassword: currentPassword,
        newPassword: newPassword,
        confirmNewPassword: newPassword
      })
    });

    const data = await response.json();

    if (response.ok) {
      console.log('Password changed successfully');
      return data;
    } else {
      console.error('Password change failed:', data.message);
      throw new Error(data.message);
    }
  } catch (error) {
    console.error('Error:', error);
    throw error;
  }
};

// Usage
changePassword('youser123', 'NewSecurePass123!')
  .then(result => console.log(result))
  .catch(error => console.error(error));
```

### C# (HttpClient)

```csharp
using System.Net.Http.Headers;
using System.Net.Http.Json;

public class ChangePasswordRequest
{
    public required string CurrentPassword { get; init; }
    public required string NewPassword { get; init; }
    public required string ConfirmNewPassword { get; init; }
}

public class ChangePasswordResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}

public async Task<ChangePasswordResponse> ChangePasswordAsync(
    string currentPassword,
    string newPassword,
    string jwtToken)
{
    using var client = new HttpClient();
    client.BaseAddress = new Uri("http://localhost:5000");
    client.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue("Bearer", jwtToken);

    var request = new ChangePasswordRequest
    {
        CurrentPassword = currentPassword,
        NewPassword = newPassword,
        ConfirmNewPassword = newPassword
    };

    var response = await client.PostAsJsonAsync("/auth/change-password", request);

    if (response.IsSuccessStatusCode)
    {
        return await response.Content.ReadFromJsonAsync<ChangePasswordResponse>()
            ?? throw new Exception("Failed to deserialize response");
    }
    else
    {
        var error = await response.Content.ReadAsStringAsync();
        throw new Exception($"Password change failed: {error}");
    }
}
```

### Python (requests)

```python
import requests

def change_password(current_password, new_password, jwt_token):
    url = "http://localhost:5000/auth/change-password"

    headers = {
        "Content-Type": "application/json",
        "Authorization": f"Bearer {jwt_token}"
    }

    data = {
        "currentPassword": current_password,
        "newPassword": new_password,
        "confirmNewPassword": new_password
    }

    response = requests.post(url, json=data, headers=headers)

    if response.status_code == 200:
        result = response.json()
        print("Password changed successfully")
        return result
    else:
        error_data = response.json()
        print(f"Password change failed: {error_data.get('message', 'Unknown error')}")
        raise Exception(error_data.get('message', 'Unknown error'))

# Usage
try:
    jwt_token = "your-jwt-token-here"
    result = change_password("youser123", "NewSecurePass123!", jwt_token)
    print(result)
except Exception as e:
    print(f"Error: {e}")
```

## Security Considerations

1. **Current Password Verification**: The endpoint requires the current password to prevent unauthorized password changes even if a token is compromised
2. **JWT Authentication**: Users must be authenticated with a valid JWT token
3. **Password Confirmation**: The new password must be provided twice to prevent typos
4. **Password Validation**: The new password must meet minimum length requirements (8-100 characters)
5. **HTTPS**: In production, always use HTTPS to protect passwords in transit

## Related Endpoints

- [Login Endpoint](./LOGIN_ENDPOINT.md) - To obtain a new JWT token after password change
- [Update Profile Endpoint](./UPDATE_PROFILE_ENDPOINT.md) - To update other profile information

## Notes

- After changing the password, the user should log in again with the new password to get a fresh JWT token
- The current JWT token remains valid until it expires naturally
- In production, consider implementing token invalidation upon password change for enhanced security

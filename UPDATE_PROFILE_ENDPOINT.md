# Update Profile Endpoint

## Overview

The Update Profile endpoint (`PUT /users/me`) allows authenticated users to update their own profile information including username, name, email, and date of birth. Users cannot change their own role through this endpoint.

## Endpoint Details

- **URL**: `/users/me`
- **Method**: `PUT`
- **Authentication**: Required (JWT Bearer token)
- **Content-Type**: `application/json`

## Request Body

```json
{
  "userName": "string (3-20 characters)",
  "firstName": "string (1-100 characters)",
  "middleName": "string (0-100 characters, optional)",
  "lastName": "string (1-100 characters)",
  "motherMaidenName": "string (0-100 characters, optional)",
  "dateOfBirth": "YYYY-MM-DD",
  "email": "string (valid email, max 255 characters)"
}
```

### Field Validation

| Field | Type | Required | Constraints |
|-------|------|----------|-------------|
| `userName` | string | Yes | 3-20 characters, must be unique |
| `firstName` | string | Yes | 1-100 characters |
| `middleName` | string | No | Max 100 characters |
| `lastName` | string | Yes | 1-100 characters |
| `motherMaidenName` | string | No | Max 100 characters |
| `dateOfBirth` | date | Yes | Format: YYYY-MM-DD |
| `email` | string | Yes | Valid email format, max 255 characters, must be unique |

## Success Response

**Code**: `200 OK`

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "userName": "johndoe",
  "firstName": "John",
  "middleName": "Michael",
  "lastName": "Doe",
  "motherMaidenName": "Smith",
  "dateOfBirth": "1990-05-15",
  "email": "john.doe@example.com",
  "role": {
    "id": "4fa85f64-5717-4562-b3fc-2c963f66afa7",
    "name": "User",
    "description": "Standard user with basic permissions"
  }
}
```

## Error Responses

### Email Already Registered

**Code**: `400 Bad Request`

```json
{
  "error": "Email address is already registered"
}
```

### Username Already Taken

**Code**: `400 Bad Request`

```json
{
  "error": "Username is already taken"
}
```

### Invalid or Missing Authentication

**Code**: `401 Unauthorized`

```json
{
  "error": "Invalid or missing user authentication"
}
```

### User Not Found

**Code**: `404 Not Found`

(Response body is empty)

### Validation Errors

**Code**: `400 Bad Request`

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Email": [
      "The Email field is not a valid e-mail address."
    ],
    "UserName": [
      "The field UserName must be a string with a minimum length of 3 and a maximum length of 20."
    ]
  }
}
```

## Examples

### cURL

```bash
curl -X PUT http://localhost:5000/users/me \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -d '{
    "userName": "johndoe",
    "firstName": "John",
    "middleName": "Michael",
    "lastName": "Doe",
    "motherMaidenName": "Smith",
    "dateOfBirth": "1990-05-15",
    "email": "john.doe@example.com"
  }'
```

### JavaScript (Fetch API)

```javascript
const updateProfile = async (profileData) => {
  try {
    const response = await fetch('http://localhost:5000/users/me', {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${localStorage.getItem('token')}`
      },
      body: JSON.stringify(profileData)
    });

    if (response.ok) {
      const data = await response.json();
      console.log('Profile updated successfully:', data);
      return data;
    } else if (response.status === 400) {
      const error = await response.json();
      console.error('Validation error:', error.error);
      throw new Error(error.error);
    } else {
      throw new Error(`HTTP error! status: ${response.status}`);
    }
  } catch (error) {
    console.error('Error updating profile:', error);
    throw error;
  }
};

// Usage
const profileData = {
  userName: 'johndoe',
  firstName: 'John',
  middleName: 'Michael',
  lastName: 'Doe',
  motherMaidenName: 'Smith',
  dateOfBirth: '1990-05-15',
  email: 'john.doe@example.com'
};

updateProfile(profileData)
  .then(result => console.log('Updated user:', result))
  .catch(error => console.error('Update failed:', error));
```

### C# (HttpClient)

```csharp
using System.Net.Http.Headers;
using System.Net.Http.Json;

public class UpdateMyProfileRequest
{
    public required string UserName { get; init; }
    public required string FirstName { get; init; }
    public string? MiddleName { get; init; }
    public required string LastName { get; init; }
    public string? MotherMaidenName { get; init; }
    public required DateOnly DateOfBirth { get; init; }
    public required string Email { get; init; }
}

public class UserResponseDto
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = string.Empty;
    public string? MotherMaidenName { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public string Email { get; set; } = string.Empty;
    public RoleDto Role { get; set; } = null!;
}

public class RoleDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public async Task<UserResponseDto> UpdateMyProfileAsync(
    UpdateMyProfileRequest request,
    string jwtToken)
{
    using var client = new HttpClient();
    client.BaseAddress = new Uri("http://localhost:5000");
    client.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue("Bearer", jwtToken);

    var response = await client.PutAsJsonAsync("/users/me", request);

    if (response.IsSuccessStatusCode)
    {
        return await response.Content.ReadFromJsonAsync<UserResponseDto>()
            ?? throw new Exception("Failed to deserialize response");
    }
    else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
    {
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        throw new Exception($"Validation error: {error?.Error ?? "Unknown error"}");
    }
    else
    {
        throw new Exception($"Update failed with status: {response.StatusCode}");
    }
}

public class ErrorResponse
{
    public string Error { get; set; } = string.Empty;
}
```

### Python (requests)

```python
import requests
from datetime import date

def update_profile(profile_data, jwt_token):
    url = "http://localhost:5000/users/me"

    headers = {
        "Content-Type": "application/json",
        "Authorization": f"Bearer {jwt_token}"
    }

    response = requests.put(url, json=profile_data, headers=headers)

    if response.status_code == 200:
        result = response.json()
        print("Profile updated successfully")
        return result
    elif response.status_code == 400:
        error_data = response.json()
        print(f"Validation error: {error_data.get('error', 'Unknown error')}")
        raise Exception(error_data.get('error', 'Unknown error'))
    else:
        print(f"Update failed with status: {response.status_code}")
        raise Exception(f"Update failed with status: {response.status_code}")

# Usage
profile_data = {
    "userName": "johndoe",
    "firstName": "John",
    "middleName": "Michael",
    "lastName": "Doe",
    "motherMaidenName": "Smith",
    "dateOfBirth": "1990-05-15",
    "email": "john.doe@example.com"
}

try:
    jwt_token = "your-jwt-token-here"
    result = update_profile(profile_data, jwt_token)
    print(f"Updated user: {result}")
except Exception as e:
    print(f"Error: {e}")
```

## Important Notes

### What Users CAN Update

- Username (must be unique)
- First name
- Middle name (optional)
- Last name
- Mother's maiden name (optional)
- Date of birth
- Email address (must be unique)

### What Users CANNOT Update

- **Role**: Users cannot change their own role. This prevents privilege escalation attacks.
- **User ID**: The ID is immutable and determined from the JWT token.

### Admin User Updates

If a SuperAdmin or Admin needs to update a user's role or other privileged information, they should use the admin endpoint:

- **Endpoint**: `PUT /users/{id}`
- **Authorization**: Admin role required
- **Can change**: All user fields including role
- **See**: Admin API documentation for details

## Security Considerations

1. **JWT Authentication**: Users must be authenticated with a valid JWT token
2. **User Identification**: User ID is extracted from the JWT token claims, not from request body
3. **Self-Service Only**: Users can only update their own profile, not other users' profiles
4. **Role Protection**: Users cannot escalate their privileges by changing their role
5. **Email Uniqueness**: Prevents duplicate email addresses in the system
6. **Username Uniqueness**: Prevents duplicate usernames in the system

## Related Endpoints

- [Change Password Endpoint](./CHANGE_PASSWORD_ENDPOINT.md) - To change password
- [Login Endpoint](./LOGIN_ENDPOINT.md) - To obtain a JWT token
- [GET /users/{id}](./README.md) - To view user details

## Use Cases

### Update Email Address

```javascript
const updateEmail = async (newEmail) => {
  // First, get current profile
  const currentProfile = await getCurrentProfile();

  // Update email
  const updatedProfile = {
    ...currentProfile,
    email: newEmail
  };

  return await updateProfile(updatedProfile);
};
```

### Update Name

```javascript
const updateName = async (firstName, lastName) => {
  const currentProfile = await getCurrentProfile();

  const updatedProfile = {
    ...currentProfile,
    firstName: firstName,
    lastName: lastName
  };

  return await updateProfile(updatedProfile);
};
```

## Testing with Development Users

Use the seed data credentials from [SEED_DATA_CREDENTIALS.md](./SEED_DATA_CREDENTIALS.md):

```bash
# Login as regular user
curl -X POST http://localhost:5000/auth/login \
  -H "Content-Type: application/json" \
  -d '{"usernameOrEmail": "user", "password": "youser123"}'

# Get JWT token from response, then update profile
curl -X PUT http://localhost:5000/users/me \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -d '{
    "userName": "user",
    "firstName": "Regular",
    "lastName": "User",
    "dateOfBirth": "1995-01-01",
    "email": "user@example.com"
  }'
```

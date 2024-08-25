"use client"

import { useEffect, useState } from 'react';

interface UserInfo {
  token: string;
}

export default function LoginCallbackPage() {
  const [userInfo, setUserInfo] = useState<UserInfo | null>(null);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchUserInfo = async () => {
      try {
        // Lấy fragment identifier từ URL (phần sau dấu #)
        const hash = window.location.hash;
        const params = new URLSearchParams(hash.substring(1)); // Bỏ dấu # và xử lý các tham số

        const idToken = params.get('id_token');
        const accessToken = params.get('access_token');
        

        if (idToken) {
          // Hiển thị thông tin token trên giao diện
          
          setUserInfo({ token: idToken });
          console.log(idToken)
          // Gửi token đến server
          const response = await fetch('http://localhost:5278/api/v1/authentication/google-callback', {
            method: 'POST',
            headers: {
              'Content-Type': 'application/json',
            },
            body: idToken, // Gửi token đến server
          });

          if (!response.ok) {
            throw new Error('Failed to send token to the server');
          }

          const data = await response.json();
          console.log('Server response:', data);

          // Xử lý thêm nếu cần (ví dụ: lưu token vào localStorage, chuyển hướng)
        } else {
          setError('No token found in URL');
        }
      } catch (err: any) {
        setError(err.message);
      }
    };

    fetchUserInfo();
  }, []);

  return (
    <div>
      <h1>Login Callback Page</h1>
        <div>
          <h2>User Info:</h2>
          {userInfo ? (
            <pre>{JSON.stringify(userInfo, null, 2)}</pre>
          ) : (
            <p>Loading...</p>
          )}
        </div>
      </div>
  );
}

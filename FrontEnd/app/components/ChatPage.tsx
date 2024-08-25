"use client"

import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import React, { useEffect, useState } from 'react';
import axios from 'axios';
import * as signalR from "@microsoft/signalr";
import { toast } from 'sonner';

interface Message {
    user: string;
    message: string;
}

export default function ChatPage() {
    const [connection, setConnection] = useState<signalR.HubConnection | null>(null);
    const [messages, setMessages] = useState<Message[]>([]);
    const [user, setUser] = useState<string>('');
    const [message, setMessage] = useState<string>('');

    useEffect(() => {
        const conn = new signalR.HubConnectionBuilder()
            .configureLogging(signalR.LogLevel.Debug)
            .withUrl("http://localhost:5278/chat-hub", {
                skipNegotiation: true,
                transport: signalR.HttpTransportType.WebSockets,
            })
            .build();

        conn.start()
            .then(() => {
                console.log("Connection started");
                toast.success("Connected to SignalR");
            })
            .catch(err => {
                console.log("Error connecting to SignalR", err);
                toast.error("Error connecting to SignalR");
            });

        conn.on("ReceiveMessage", (user: string, message: string) => {
            const newMessage = { user, message };
            setMessages(messages => [...messages, newMessage]);
            toast.info(`${user}: ${message}`);
        });

        setConnection(conn);

        return () => {
            conn.stop();
        };
    }, []);

    const sendMessage = async () => {
        if (connection && user && message) {
            try {
                await connection.invoke("SendMessage", user, message);
                setMessage('');
            } catch (err) {
                console.error("Error sending message", err);
                toast.error("Error sending message");
            }
        }
    };

    const handleGoogleLogin = async () => {
        try {
            const clientId = '1097822686619-u8ekfqo6bb9q4lsqv4kbp99sqaai0ee0.apps.googleusercontent.com';
            const redirectUri = 'http://localhost:3000/login/callback'; // Thay thế bằng callback của bạn

            // them nonce để chống CSRF
            const scope = 'profile email';
            const responseType = 'id_token token';
            const nonce = '123456';

            // Xây dựng URL yêu cầu Google OAuth 2.0
            const googleLoginUrl = `https://accounts.google.com/o/oauth2/v2/auth?client_id=${clientId}&redirect_uri=${redirectUri}&response_type=${responseType}&scope=${scope}&nonce=${nonce}`;

            // Mở trang đăng nhập Google
            window.location.href = googleLoginUrl;
        } catch (error) {
            console.error("Error during Google login", error);
            toast.error("Error during Google login");
        }
    };

    return (
        <div className='p-4 space-y-4'>
            <h1>ChatPage</h1>
            <div>
                <p>Chat with your friends</p>
                <h2>User Info</h2>
                <p>Username: John Doe</p>
                <p>Email: uydev@gmail.com</p>
                <p>Avatar: 
                    <img src="https://avatars.githubusercontent.com/u/36305929?v=4" alt="avatar" className='h-10 w-10'/>
                </p>
            </div>

            {/* login gg btn */}
            <Button type="button" onClick={handleGoogleLogin}>Login with Google</Button>

            {/* chat */}
            <div className='global-chat'>
                <div className="space-y-2">
                    <div className="flex items-center space-x-2">
                        <label htmlFor="userInput" className="w-20">User</label>
                        <Input 
                            type="text" 
                            id="userInput" 
                            value={user}
                            onChange={(e) => setUser(e.target.value)}
                            className="flex-1"
                        />
                    </div>
                    <div className="flex items-center space-x-2">
                        <label htmlFor="messageInput" className="w-20">Message</label>
                        <Input 
                            type="text" 
                            id="messageInput" 
                            value={message}
                            onChange={(e) => setMessage(e.target.value)}
                            className="flex-1"
                        />
                    </div>
                    <div className="text-right">
                        <Button type="button" id="sendButton" onClick={sendMessage}>
                            Send
                        </Button>
                    </div>
                </div>

                <div className="mt-4">
                    <hr />
                </div>

                <div className="mt-4">
                    <ul id="messagesList" className="space-y-2">
                        {messages.map((msg, index) => (
                            <li key={index} className="border p-2 rounded-md">
                                <strong>{msg.user}</strong>: {msg.message}
                            </li>
                        ))}
                    </ul>
                </div>
            </div>
        </div>
    );
}

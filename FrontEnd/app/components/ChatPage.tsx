"use client"

import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import * as signalR from "@microsoft/signalr";
import axios from 'axios';
import { format } from 'date-fns';
import { useEffect, useState } from 'react';
import { toast } from 'react-toastify';

interface Message {
    chatRoomId: string;
    userId: string;
    content: string;
    createdAt: string;
}

export default function ChatPage() {
    const [connection, setConnection] = useState<signalR.HubConnection | null>(null);
    const [messages, setMessages] = useState<Message[]>([]);
    const [user, setUser] = useState<string>('');
    const [message, setMessage] = useState<string>('');
    const [email, setEmail] = useState<string>('');
    const [otp, setOtp] = useState<string>('');
    const [isAuthenticated, setIsAuthenticated] = useState<boolean>(false);

    useEffect(() => {
        const conn = new signalR.HubConnectionBuilder()
            .configureLogging(signalR.LogLevel.Debug)
            .withUrl("https://localhost:5278/chat-hub", {
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

        conn.on("LoadRecentMessages", (messages) => {
            console.log("Recent messages received:", messages);
            try {
                setMessages(messages);
            } catch (error) {
                console.error("Error parsing JSON:", error);
            }
        });

        conn.on("ReceiveMessage", (message: any) => {
            console.log(message);

            const newMessage = {
                chatRoomId: message?.chatRoomId,
                userId: message?.userId,
                content: message?.content,
                createdAt: new Date().toISOString(),
             };
            setMessages(messages => [...messages, newMessage]);
            toast.info(`${user}: ${message}`);
        });

        setConnection(conn);

        return () => {
            conn.stop();
        };
    }, []);

    const sendMessage = async () => {
        console.log("Sending message", connection, user, message);
        if (connection && user && message) {
            try {
                console.log("Sending message", message);
                await connection.invoke("SendMessage", message);
                setMessage('');
            } catch (err) {
                console.error("Error sending message", err);
                toast.error("Error sending message");
            }
        }
    };

    const handleRequestOtp = async () => {
        try {
            const response = await axios.post('https://localhost:5278/api/v1/auth/otp', {
                email,
                name: user || "Anonymous"
            });

            if (response.data.success) {
                toast.success("OTP sent successfully. Please check your email.");
            } else {
                toast.error("Failed to send OTP.");
            }
        } catch (error) {
            console.error("Error requesting OTP", error);
            toast.error("Error requesting OTP");
        }
    };

    const handleVerifyOtp = async () => {
        try {
            const response = await axios.post('https://localhost:5278/api/v1/auth/otp/verify', {
                email,
                otpCode: otp
            });

            if (response.data.success) {
                setIsAuthenticated(true);
                toast.success("OTP verified successfully.");
            } else {
                toast.error("Invalid OTP.");
            }
        } catch (error) {
            console.error("Error verifying OTP", error);
            toast.error("Error verifying OTP");
        }
    };

    return (
        <div className="p-6 space-y-6">
            <h1 className="text-3xl font-bold">Chat Page</h1>
            {!isAuthenticated ? (
                <div className="space-y-4">
                    <h2 className="text-xl font-semibold">Login with OTP</h2>
                    <div className="flex flex-col space-y-4">
                        <Input 
                            type="text" 
                            placeholder="Enter your email" 
                            value={email}
                            onChange={(e) => setEmail(e.target.value)}
                        />
                        <Button type="button" onClick={handleRequestOtp}>
                            Request OTP
                        </Button>
                        <Input 
                            type="text" 
                            placeholder="Enter your OTP" 
                            value={otp}
                            onChange={(e) => setOtp(e.target.value)}
                        />
                        <Button type="button" onClick={handleVerifyOtp}>
                            Verify OTP
                        </Button>
                    </div>
                </div>
            ) : (
                <>Login rồi</>
            )}

                <div>
                    <div className="space-y-2">
                        {
                            isAuthenticated ? (
                                <div>
                                    <h2 className="text-xl font-semibold">Authenticated User</h2>
                                    <p className="text-lg">User: {user}</p>
                                </div>
                            ) : 
                            <div>
                                <h2 className="text-xl font-semibold">Unauthenticated User</h2>
                                <p className="text-lg">User: Anonymous</p>
                            </div>
                        }
                    </div>
                    <div className="mt-6 space-y-4">
                        <h2 className="text-xl font-semibold">Global Chat</h2>
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
                        {/* title */}
                        <h2 className="text-xl font-semibold">Messages</h2>
                        <div className="mt-4">
                            <ul id="messagesList" className="space-y-2">
                                {messages.map((msg, index) => (
                                    <li key={index} className="border p-2 rounded-md">
                                        <strong>{msg.userId}</strong>: {msg.content}
                                        <div className="text-xs text-gray-500">{
                                            format(new Date(msg.createdAt), 'yyyy-MM-dd HH:mm:ss')
                                            }</div>
                                    </li>
                                ))}
                            </ul>
                        </div>
                    </div>
                </div>
        </div>
    );
}

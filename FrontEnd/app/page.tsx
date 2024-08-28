"use client"

import React from 'react'
import ChatPage from './components/ChatPage'
import { ToastContainer } from 'react-toastify'

export default function HomePage() {
  return (
    <div>
      <h1>HomePage</h1>
      <ChatPage />
      <ToastContainer />
    </div>
  )
}

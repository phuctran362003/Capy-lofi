import './globals.css';
import './toast.css';

import { Analytics } from '@vercel/analytics/react';

export const metadata = {
  title: 'Next.js App Router + Tailwind CSS',
  description:
    ''
};

export default function RootLayout({
  children
}: {
  children: React.ReactNode;
}) {
  return (
    <html lang="en">
      <body className="flex min-h-screen w-full flex-col">{children}</body>
      <Analytics />
    </html>
  );
}

# JusGiveaway App 🎉
Welcome to JusGiveaway, a Blazor-based web app where users can engage in fun activities and win exciting prizes through daily or seasonal giveaways. This application provides a seamless and engaging user experience with interactive features and a clean design.

## Description
Official repo for the development and maintenance of JusGiveaway web app.

## Features 🌟
- **User-Friendly Sign-Up/Sign-In**: Supports authentication with Firebase for secure and smooth user management.
- **Referral System**: Users can invite others using personalized referral codes.
- **Daily Giveaways**: Engage in daily activities for a chance to win.
- **Dynamic Visual Effects**: Includes animations like coin flips, naira rain, and particle effects for wins, losses, and special prizes.
- **Responsive Design**: Optimized for desktop and mobile platforms.
- **Offline Support**: IndexedDB integration for storing user information locally.
- **Online Support**: Firebase integration for storing user information in the cloud.

## Technology Stack 🛠️
- **Frontend**: HTML, CSS, JS (Blazor)
- **Backend**: C# (Blazor)
- **Database**: IndexedDB (for local storage), Firebase (for online)
- **Deployment**: Hosted on a web server

## Getting Started 🚀
### Prerequisites
- .NET SDK for Blazor development.
- A Firebase project configured for authentication and database usage.

## Setup & Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/yourusername/jusgiveaway.git
   cd jusgiveaway
2. Run project locally
3. Access app at localhost

## Deployment 📦
### Manual Deployment with FileZilla (currently)
- Build the app in release mode:
- Copy the contents of the publish folder to your web server using FileZilla or another FTP client.
- Backup the current version of your site before uploading new files.

### Future Plans for CI/CD
Looking into automated deployment pipelines using GitHub Actions or similar tools for more efficiency.

## Contributing 🤝
We will be opening this up to contributions soon!

## Known Issues ⚠️
- App doesn’t load on older iOS devices running iOS 16.
- Coin-flip animations behave inconsistently on different browsers.


# 🚜 BlazorVille
A high-performance, persistent farming simulator built with C# and Blazor WebAssembly. This project recreates the classic "social farming" loop of the 2000s using modern web technologies.

## 🎮 Game Features
- **Persistent Farm Slots**: Support for up to 5 individual save slots.

- **State-Driven Gameplay**: * 
  - Plow: Prepare grass for planting (Cost: 15 coins).

  - **Plant**: Purchase seeds from the Market and watch them grow in real-time.

  - **Harvest**: Collect fully grown crops for profit and XP.

  - **Wither System**: If neglected, crops will wither and must be cleared.

- **Tool-Based Interaction**: Switch between a Pointer, Plow, and Rake to manage your land.

- **PWA Ready**: Installable on Desktop and Mobile with offline support and a custom splash screen.

## 🛠️ Technical Stack
- **Frontend**: Blazor WebAssembly (.NET 8.0/9.0)

- **Persistence**: Browser LocalStorage via a decoupled FarmService.

- **Styling**: CSS Grid for the farm layout and CSS Custom Properties for framework-integrated loading bars.

- **Lifecycle**: System.Timers and StateHasChanged heartbeat for real-time growth calculations.
  
## 📂 Project Structure
```text
Farmville.Client/
├── Models/
│   ├── Crop.cs             # Data structure for plants
│   ├── FarmTile.cs         # Logic for growth and states
│   └── FarmSaveData.cs     # The object we serialize to JSON
├── Services/
│   └── FarmService.cs      # Handles LocalStorage and save slots
├── Pages/
│   ├── Index.razor          # Main Menu / Landing Page
│   ├── Dashboard.razor      # Save Slot selection (1-5)
│   ├── Farm.razor           # The main game grid and HUD
│   └── Market.razor         # The plant shop modal
├── Shared/
│   └── MainLayout.razor     # Global UI (Sidebar/Navbar)
└── wwwroot/
    ├── css/
    │   └── app.css          # All game and HUD styling
    ├── manifest.json        # PWA configuration
    ├── service-worker.js    # Offline/Caching logic
    └── index.html           # Splash screen & Loading bar
```

## 🚀 Installation & Local Setup
1. Prerequisites: Install the .NET SDK.

2. Clone & Run:

```Bash
dotnet watch run
```
3. PWA Installation: * Open the app in Chrome or Edge.
- Click the Install icon in the address bar to run it as a standalone application.

## 📝 Roadmap
- [ ] XP Leveling: Unlock higher-tier crops (Corn, Pumpkins) at specific levels.

- [ ] Animations: Floating "+5 XP" text on harvest and "Level Up" fanfares.

- [ ] Art Assets: Replace Emojis with custom Sprite Sheets.

- [ ] Watering Mechanic: Add a Watering Can tool to speed up growth rates.

## ⚖️ License
MIT - Feel free to use this as a foundation for your own Blazor game projects!
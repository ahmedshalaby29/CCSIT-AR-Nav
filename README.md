
# 📍 CCSIT-AR-Nav

**CCSIT-AR-Nav** is an **Augmented Reality navigation system** built with **Unity and Vuforia**.  
The project is designed to help users (e.g., students and visitors) navigate the **College of Computer Science and Information Technology (CCSIT)** campus through AR markers and interactive overlays.

---

## ✨ Features
- 🔎 **AR Marker Recognition** – Uses Vuforia to detect and track predefined images or objects.
- 🧭 **AR Navigation** – Provides directional guidance with overlays in real time.
- 📱 **Mobile Ready** – Build and deploy for Android (and iOS if needed).
- 🗂️ **Scalable Setup** – New targets can be easily added to expand navigation points.
- 🛠️ **Unity Integration** – Simple to modify, extend, and customize in the Unity Editor.

---

## 🖼️ Demo

https://github.com/user-attachments/assets/269780c1-8550-49ce-8582-9535cb9fda26


---

## 🚀 Getting Started

### Prerequisites
- [Unity Hub](https://unity.com/download) (recommend LTS version, e.g., 2021 LTS or later)
- [Vuforia Engine](https://developer.vuforia.com/) SDK
- Android/iOS build support in Unity

### Setup Instructions
1. **Clone the Repository**
   ```bash
   git clone https://github.com/ahmedshalaby29/CCSIT-AR-Nav.git
   ```
2. **Open in Unity**
   - Launch Unity Hub
   - Add the project folder
   - Open it with a compatible Unity version
3. **Enable Vuforia**
   - Go to `Edit > Project Settings > XR Plug-in Management`
   - Enable **Vuforia Engine AR**
4. **Add Your License Key**
   - Sign up at [Vuforia Developer Portal](https://developer.vuforia.com/)
   - Create a license key
   - In Unity: go to `Window > Vuforia Engine > Configuration` and paste your key
5. **Build & Run**
   - Connect your Android/iOS device
   - Go to `File > Build Settings`
   - Choose the target platform and click **Build & Run**

---

## 📂 Project Structure
- `Assets/` – Unity project assets, AR targets, scripts, and scenes
- `Packages/` – Package dependencies
- `ProjectSettings/` – Unity project configuration

---

## ⚠️ Notes
- Make sure you have a stable internet connection if using dynamic data sources.
- Test markers under good lighting for best recognition results.

---

## 📜 License
This project is licensed under the **MIT License** – see the [LICENSE](LICENSE) file for details.

---

## 👤 Author
**Ahmed Shalaby**  
- GitHub: [@ahmedshalaby29](https://github.com/ahmedshalaby29)

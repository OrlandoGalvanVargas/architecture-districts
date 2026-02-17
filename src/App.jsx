import "./App.css";
import { NotificationProvider } from "./contexts/Notification";
import { AppRouter } from "./router/AppRouter";

function App() {
  return (
    <NotificationProvider>
      <AppRouter />
    </NotificationProvider>
  );
}

export default App;

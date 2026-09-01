// src/App.jsx

import { AppRouter } from "@/router/AppRouter";
import { ErrorBoundary } from "@/components/common/ErrorBoundary/ErrorBoundary";

function App() {
  return (
    <ErrorBoundary>
      <AppRouter />
    </ErrorBoundary>
  );
}

export default App;

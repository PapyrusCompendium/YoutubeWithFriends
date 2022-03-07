import "./App.css";
import { BrowserRouter, Routes, Route } from "react-router-dom";

import LandingPage from "./pages/LandingPage";
import WatchParty from "./pages/WatchParty";

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route index element={<LandingPage />} />
        <Route path="/WatchParty" element={<WatchParty />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;

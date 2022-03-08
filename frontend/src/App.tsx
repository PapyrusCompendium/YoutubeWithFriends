import "./App.css";
import { BrowserRouter, Routes, Route } from "react-router-dom";
import { ChakraProvider } from "@chakra-ui/react";

import LandingPage from "./pages/LandingPage";
import WatchParty from "./pages/WatchParty";
import CreateUser from "./pages/CreateUser";
import { theme } from "./constants/theme";
import Layout from "./layouts/Layout";

function App() {
  return (
    <ChakraProvider theme={theme}>
      <BrowserRouter>
        <Routes>
          <Route path="/" element={<Layout />}>
            <Route index element={<LandingPage />} />
            <Route path="/WatchParty" element={<WatchParty />} />
            <Route path="/CreateUser" element={<CreateUser />} />
            <Route path="*" element={<LandingPage />} />
          </Route>
        </Routes>
      </BrowserRouter>
    </ChakraProvider>
  );
}

export default App;

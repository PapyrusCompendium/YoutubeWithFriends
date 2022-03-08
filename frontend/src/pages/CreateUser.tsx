import { useState, FormEvent } from "react";
import {
  FormControl,
  Input,
  FormErrorMessage,
  Button,
  VStack,
  Text,
} from "@chakra-ui/react";

import Cenetered, { CENTERED } from "../components/Centered";

const VALID_REGEX_MATCH = /[a-zA-Z0-9]+$/;

export default function CreateUser() {
  const [username, setUsername] = useState("");
  const [disabled, setDisabled] = useState(false);
  const [error, setError] = useState("");

  const submit = async (event: FormEvent) => {
    event.preventDefault();

    setDisabled(true);
    setError("");

    if (!username.match(VALID_REGEX_MATCH)) {
      setError("Username should only contain letters and numbers.");
    }

    // TODO: Form API submission

    setDisabled(false);
  };

  return (
    <Cenetered centered={CENTERED.BOTH}>
      <form noValidate onSubmit={submit} style={{ width: "25%" }}>
        <VStack spacing="4">
          <Text>Usernames expire after 12 hours of no activity.</Text>
          <FormControl isRequired isInvalid={!!error}>
            <Input
              type="text"
              maxLength={128}
              placeholder="Username"
              value={username}
              onChange={(event) => setUsername(event.target.value)}
              width="100%"
              autoFocus
            />
            {error && <FormErrorMessage>{error}</FormErrorMessage>}
          </FormControl>
          <Button
            disabled={disabled}
            colorScheme="brand"
            width="100%"
            type="submit"
          >
            Create User
          </Button>
        </VStack>
      </form>
    </Cenetered>
  );
}

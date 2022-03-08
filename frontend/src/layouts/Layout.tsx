import { Outlet, Link } from "react-router-dom";
import { Flex, Text, Link as ChakraLink, Box, HStack } from "@chakra-ui/react";

interface ILink {
  text: string;
  path: string;
}
const links: ILink[] = [
  {
    text: "Login",
    path: "/createUser",
  },
  {
    text: "Watch Party",
    path: "/watchParty",
  },
];

export default function Layout() {
  return (
    <Flex width="100%" height="100%" flexDirection="column">
      <Flex
        width="100%"
        px="18"
        py="4"
        flexDirection="row"
        justifyContent="space-between"
        alignItems="center"
        backgroundColor="brand.100"
      >
        <Box>
          <Text fontSize="2xl">
            <Link to="/">YouTube with Friends</Link>
          </Text>
        </Box>
        <HStack spacing="8">
          {links.map((link) => (
            <Link key={link.path} to={link.path}>
              <Text>{link.text}</Text>
            </Link>
          ))}
        </HStack>
      </Flex>
      <Flex flex={1}>
        <Outlet />
      </Flex>
    </Flex>
  );
}

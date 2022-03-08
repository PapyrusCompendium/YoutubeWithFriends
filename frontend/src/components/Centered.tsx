import { ReactNode, createElement } from "react";
import { Flex, StyleProps } from "@chakra-ui/react";

export enum CENTERED {
  VERTICAL,
  HORIZONTAL,
  BOTH,
}

interface Props {
  centered?: CENTERED;
  children: ReactNode;
}

const VERTICAL_CENTERED_STYLES: StyleProps = {
  justifyContent: "center",
};
const HORIZONTAL_CENTERED_STYLES: StyleProps = {
  alignItems: "center",
};
const BOTH_CENTERED_STYLES: StyleProps = {
  justifyContent: "center",
  alignItems: "center",
};

export default function Cenetered({
  centered = CENTERED.BOTH,
  children,
}: Props) {
  let styles = BOTH_CENTERED_STYLES;

  if (centered === CENTERED.HORIZONTAL) {
    styles = HORIZONTAL_CENTERED_STYLES;
  } else if (centered === CENTERED.VERTICAL) {
    styles = VERTICAL_CENTERED_STYLES;
  } else {
    styles = BOTH_CENTERED_STYLES;
  }

  return (
    <Flex width="100%" height="100%" flexDirection="column" {...styles}>
      {children}
    </Flex>
  );
}

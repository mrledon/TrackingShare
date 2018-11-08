import React, { Component } from "react";
import { StyleSheet } from 'react-native';
import { Button, Text } from "native-base";
import { COLORS, FONTS } from '../utils';

class MainButton extends Component {
    render() {

        const { onPress, title, style } = this.props;

        return (
            <Button
                block
                onPress={onPress}
                style={[styles.button, style]}>
                <Text uppercase={false} style={styles.title}>{title}</Text>
            </Button>
        );
    }
}

const styles = StyleSheet.create({
    button: {
        marginTop: 10,
        backgroundColor: COLORS.BLUE_2E5665,
    },
    title: {
        fontFamily: FONTS.MAIN_FONT_REGULAR
    }
})

export { MainButton };
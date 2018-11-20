import React, { Component } from "react";
import { StyleSheet, View } from 'react-native';
import { Button, Text, Icon } from "native-base";
import { COLORS, FONTS } from '../utils';

class MainButton extends Component {
    render() {

        const { onPress, title, style, isIcon, icon, size } = this.props;

        return (
            <Button
                block
                onPress={onPress}
                style={[styles.button, style]}>
                {
                    isIcon ?
                    <Icon name={icon} size={size}></Icon>:
                    <View/>
                }
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
        fontFamily: FONTS.MAIN_FONT_REGULAR,
        textAlign: 'center'
    }
})

export { MainButton };
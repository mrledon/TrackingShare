import React, { Component } from "react";
import { Button, Text } from "native-base";
import { View, StyleSheet } from "react-native";
import { COLORS, FONTS } from '../utils';

class EmptyData extends Component {
    render() {
        return (
            <View padder style={styles.container}>
                <Text style={styles.text}>{this.props.title}</Text>
                <View style={styles.subContainer}>
                    <Button onPress={this.props.onPress} style={styles.button}>
                        <Text style={styles.text}>{this.props.action}</Text>
                    </Button>
                </View>
            </View>
        );
    }
}

const styles = StyleSheet.create({
    container: {
        flex: 1,
        justifyContent: 'center',
        alignItems: 'center',
        padding: 50
    },
    subContainer: {
        paddingTop: 10
    },
    text: {
        textAlign: 'center',
        fontFamily: FONTS.MAIN_FONT_REGULAR
    },
    button: {
        height: 40,
        backgroundColor: COLORS.BLUE_2E5665
    }
})

export { EmptyData };
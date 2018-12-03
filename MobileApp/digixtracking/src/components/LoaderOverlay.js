import React, { Component } from 'react';
import { ActivityIndicator, View, StyleSheet } from 'react-native';
import { COLORS } from '../utils';

class LoaderOverlay extends Component {
    constructor(props) {
        super(props);
    }

    render() {
        return (
            <View style={styles.container}>
                <View style={styles.subcontainer}>
                    <ActivityIndicator
                        color={COLORS.BLUE_2E5665}
                        size="large"
                    />
                </View>
            </View>
        )
    }
}

const styles = StyleSheet.create({
    container: {
        flex: 1,
        justifyContent: 'center',
        alignItems: 'center',
        backgroundColor: 'transparent'
    },
    subcontainer: {
        justifyContent: 'center',
        alignItems: 'center',
        shadowOffset: {
            width: 0,
            height: 1
        },
        shadowRadius: 2,
        shadowOpacity: 0.16,
        elevation: 1,
    }
})

export default LoaderOverlay;
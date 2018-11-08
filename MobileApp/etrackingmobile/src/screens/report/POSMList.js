import React, { Component } from 'react';
import { View, StyleSheet, Image, TouchableOpacity, Alert, AsyncStorage } from 'react-native';
import { Text } from 'native-base';
import { MainButton, MainHeader } from '../../components';
import { COLORS, FONTS, STRINGS } from '../../utils';

class POSMList extends Component {
    constructor(props) {
        super(props);
        this.state = {
            list: [
                {
                    title: 'POSM Loại 1',
                    type: '1'
                },
                {
                    title: 'POSM Loại 2',
                    type: '2'
                },
                {
                    title: 'POSM Loại 3',
                    type: '3'
                },
                {
                    title: 'POSM Loại 4',
                    type: '4'
                },
                {
                    title: 'POSM Loại 5',
                    type: '5'
                },
                {
                    title: 'POSM Loại 6',
                    type: '6'
                },
            ]
        };
    }

    handlePOSMPress = () => {
        this.props.navigation.navigate('POSMDetail');
    }

    handleBack = () => {
        this.props.navigation.navigate('Home');
    }

    render() {
        return (
            <View
                style={styles.container}>
                <MainHeader
                    onPress={() => this.handleBack()}
                    hasLeft={true}
                    title={STRINGS.POSMListTitle} />
                <View
                    padder
                    style={styles.subContainer}>

                    {this.state.list.map((item, index) => {
                        return (
                            <MainButton
                                style={styles.button}
                                title={item.title}
                                onPress={() => this.handlePOSMPress()} />
                        );
                    })}

                </View>

            </View>
        );
    }
}

const styles = StyleSheet.create({
    container: {
        flex: 1
    },
    subContainer: {
        flex: 1,
        flexDirection: 'column',
        alignItems: 'center',
        maxWidth: 500,
        padding: 20
    },
    bottomContainer: {
        flexDirection: 'column',
        justifyContent: 'center',
        alignContent: 'center',
        paddingBottom: 50
    },
    button: {
        height: 50
    },
    logo: {
        marginBottom: 20,
        width: 100,
        height: 100
    },
    textBottom: {
        textAlign: 'center',
        fontFamily: FONTS.MAIN_FONT_REGULAR,
    }
});

export default POSMList;

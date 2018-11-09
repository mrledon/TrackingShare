import React, { Component } from 'react';
import { View, StyleSheet, Image, TouchableOpacity, Alert, AsyncStorage } from 'react-native';
import { Text, Item, Input } from 'native-base';
import { MainButton, MainHeader } from '../../components';
import { COLORS, FONTS, STRINGS } from '../../utils';

class StoreList extends Component {
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
                    title={'Danh sách cửa hàng'} />
                <View
                    padder
                    style={styles.subContainer}>

                    <View style={styles.centerContainer}>
                        <Text style={styles.itemSubTitle} uppercase>{'Ngày 10/08/2018'}</Text>
                    </View>

                    <View style={styles.rowContainer}>
                        <View style={styles.leftItem}>
                            <Text style={styles.title}>{'Cửa hàng: '}</Text>
                        </View>
                        <View style={styles.rightItem}>
                            <Text style={styles.title}>{'CH001 - Đại lý Minh Thành'}</Text>
                        </View>
                    </View>

                    <View style={styles.rowContainer}>
                        <View style={styles.leftItem}>
                            <Text style={styles.title}>{'Cửa hàng: '}</Text>
                        </View>
                        <View style={styles.rightItem}>
                            <Text style={styles.title}>{'CH001 - Đại lý Minh Thành'}</Text>
                        </View>
                    </View>

                    <View style={styles.rowContainer}>
                        <View style={styles.leftItem}>
                            <Text style={styles.title}>{'Cửa hàng: '}</Text>
                        </View>
                        <View style={styles.rightItem}>
                            <Text style={styles.title}>{'CH001 - Đại lý Minh Thành'}</Text>
                        </View>
                    </View>


                    <View style={styles.centerContainer}>
                        <Text style={styles.itemSubTitle} uppercase>{'Ngày 11/08/2018'}</Text>
                    </View>

                    <View style={styles.rowContainer}>
                        <View style={styles.leftItem}>
                            <Text style={styles.title}>{'Cửa hàng: '}</Text>
                        </View>
                        <View style={styles.rightItem}>
                            <Text style={styles.title}>{'CH001 - Đại lý Minh Thành'}</Text>
                        </View>
                    </View>

                    <View style={styles.rowContainer}>
                        <View style={styles.leftItem}>
                            <Text style={styles.title}>{'Cửa hàng: '}</Text>
                        </View>
                        <View style={styles.rightItem}>
                            <Text style={styles.title}>{'CH001 - Đại lý Minh Thành'}</Text>
                        </View>
                    </View>

                    <View style={styles.rowContainer}>
                        <View style={styles.leftItem}>
                            <Text style={styles.title}>{'Cửa hàng: '}</Text>
                        </View>
                        <View style={styles.rightItem}>
                            <Text style={styles.title}>{'CH001 - Đại lý Minh Thành'}</Text>
                        </View>
                    </View>

                    <View style={styles.centerContainer}>
                        <Text style={styles.itemSubTitle} uppercase>{'Ngày 12/08/2018'}</Text>
                    </View>

                    <View style={styles.rowContainer}>
                        <View style={styles.leftItem}>
                            <Text style={styles.title}>{'Cửa hàng: '}</Text>
                        </View>
                        <View style={styles.rightItem}>
                            <Text style={styles.title}>{'CH001 - Đại lý Minh Thành'}</Text>
                        </View>
                    </View>

                    <View style={styles.rowContainer}>
                        <View style={styles.leftItem}>
                            <Text style={styles.title}>{'Cửa hàng: '}</Text>
                        </View>
                        <View style={styles.rightItem}>
                            <Text style={styles.title}>{'CH001 - Đại lý Minh Thành'}</Text>
                        </View>
                    </View>

                    <View style={styles.rowContainer}>
                        <View style={styles.leftItem}>
                            <Text style={styles.title}>{'Cửa hàng: '}</Text>
                        </View>
                        <View style={styles.rightItem}>
                            <Text style={styles.title}>{'CH001 - Đại lý Minh Thành'}</Text>
                        </View>
                    </View>
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
        alignItems: 'flex-start',
        justifyContent: 'flex-start',
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
    },
    centerContainer: {
        alignItems: 'center',
        justifyContent: 'center',
        flexDirection: 'column',
        paddingTop: 20
    },
    itemSubTitle: {
        fontFamily: FONTS.MAIN_FONT_BOLD,
        marginBottom: 15
    },
    rowContainer: {
        alignItems: 'center',
        justifyContent: 'center',
        flexDirection: 'row',
        padding: 3
    },
    leftItem: {
        flex: 0.3,
        justifyContent: 'center',
        alignContent: 'center',
    },
    rightItem: {
        flex: 0.7,
        justifyContent: 'center',
        alignContent: 'center',
    },
    item: {
        height: 40,
        marginBottom: 20
    },
    input: {
        fontFamily: FONTS.MAIN_FONT_REGULAR
    },
});

export default StoreList;

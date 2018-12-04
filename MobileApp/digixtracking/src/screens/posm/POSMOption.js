import React, { Component } from 'react';
import { View, StyleSheet, TouchableOpacity, Alert, AsyncStorage, ScrollView, Dimensions } from 'react-native';
import { Text, Item } from 'native-base';
import { MainButton, MainHeader } from '../../components';
import { COLORS, FONTS, STRINGS } from '../../utils';

import { bindActionCreators } from "redux";
import { connect } from "react-redux";

const { width, height } = Dimensions.get("window");

const CARD_WIDTH = width - 40;

class POSMOption extends Component {
    constructor(props) {
        super(props);
        this.state = {
            data: [
                {
                    title: 'Ảnh cửa hàng',
                    screen: 'POSMTakePhoto',
                    type: 'DEFAULT'
                },
                {
                    title: 'Tranh Pepsi & 7Up',
                    screen: 'POSMTakePhoto',
                    type: 'TRANH_PEPSI_AND_7UP'
                },
                {
                    title: 'Sticker 7Up',
                    screen: 'POSMTakePhoto',
                    type: 'STICKER_7UP'
                },
                {
                    title: 'Sticker Pepsi',
                    screen: 'POSMTakePhoto',
                    type: 'STICKER_PEPSI'
                },
                {
                    title: 'Banner Pepsi',
                    screen: 'POSMTakePhoto',
                    type: 'BANNER_PEPSI'
                },
                {
                    title: 'Banner 7Up Tết',
                    screen: 'POSMTakePhoto',
                    type: 'BANNER_7UP_TET'
                },
                {
                    title: 'Banner Mirinda',
                    screen: 'POSMTakePhoto',
                    type: 'BANNER_MIRINDA'
                },
                {
                    title: 'Banner Twister',
                    screen: 'POSMTakePhoto',
                    type: 'BANNER_TWISTER'
                },
                {
                    title: 'Banner Revive',
                    screen: 'POSMTakePhoto',
                    type: 'BANNER_REVIVE'
                },
                {
                    title: 'Banner Olong',
                    screen: 'POSMTakePhoto',
                    type: 'BANNER_OOLONG'
                },
            ]
        };
    }

    handleGoto = (screen, type) => {
        this.props.navigation.navigate(screen, {
            type: type
        });
    }

    handleDone = async () => {

        try {
            const _data = await AsyncStorage.getItem('DATA_SSC');

            const _posm = await AsyncStorage.getItem('POSM_SSC');

            if (_posm != null) {
                let _posmParse = JSON.parse(_posm);

                if (_data != null) {
                    let dataParse = JSON.parse(_data);
                    dataParse.push(_posmParse);
                    await AsyncStorage.setItem('DATA_SSC', JSON.stringify(dataParse));
                }
                else {
                    var _newData = [];
                    _newData.push(_posmParse);
                    await AsyncStorage.setItem('DATA_SSC', JSON.stringify(_newData));
                }

                Alert.alert(
                    STRINGS.MessageTitleAlert, 'Lưu dữ liệu thành công',
                    [{
                        text: STRINGS.MessageActionOK, onPress: () => {

                            this.props.navigation.navigate('Home');
                        }
                    }],
                    { cancelable: false }
                );
            }
        } catch (error) {
            Alert.alert('Lỗi');
        }
    }

    render() {

        const { data } = this.state;

        return (
            <View
                style={styles.container}>
                <MainHeader
                    hasLeft={false}
                    title={'Chọn loại ảnh'} />
                <View
                    padder
                    style={styles.subContainer}>

                    <ScrollView
                        horizontal={false}
                        showsHorizontalScrollIndicator={false}
                        style={{ padding: 0, width: CARD_WIDTH, marginBottom: 20 }}>
                        {data.map((item, index) => {
                            return (
                                <MainButton
                                    style={styles.button}
                                    title={item.title.toLocaleUpperCase()}
                                    onPress={() => this.handleGoto(item.screen, item.type)} />
                            );
                        })}
                    </ScrollView>

                    <MainButton
                        style={styles.button2}
                        title={'HOÀN THÀNH'}
                        onPress={this.handleDone} />

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
        padding: 20
    },
    bottomContainer: {
        flexDirection: 'column',
        justifyContent: 'center',
        alignContent: 'center',
        paddingBottom: 50,
    },
    button: {
        height: 50
    },
    button2: {
        height: 50,
        backgroundColor: 'green'
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

function mapStateToProps(state) {
    return {
    }
}

function dispatchToProps(dispatch) {
    return bindActionCreators({
    }, dispatch);
}

export default connect(mapStateToProps, dispatchToProps)(POSMOption);

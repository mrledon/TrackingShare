import React, { Component } from 'react';
import { View, StyleSheet, TouchableOpacity, Alert, AsyncStorage, ScrollView, Dimensions, BackHandler } from 'react-native';
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
                    type: 'DEFAULT',
                    count: 0
                },
                {
                    title: 'Tranh Pepsi & 7Up',
                    screen: 'POSMTakePhoto',
                    type: 'TRANH_PEPSI_AND_7UP',
                    count: 0
                },
                {
                    title: 'Sticker 7Up',
                    screen: 'POSMTakePhoto',
                    type: 'STICKER_7UP',
                    count: 0
                },
                {
                    title: 'Sticker Pepsi',
                    screen: 'POSMTakePhoto',
                    type: 'STICKER_PEPSI',
                    count: 0
                },
                {
                    title: 'Banner Pepsi',
                    screen: 'POSMTakePhoto',
                    type: 'BANNER_PEPSI',
                    count: 0
                },
                {
                    title: 'Banner 7Up Tết',
                    screen: 'POSMTakePhoto',
                    type: 'BANNER_7UP_TET',
                    count: 0
                },
                {
                    title: 'Banner Mirinda',
                    screen: 'POSMTakePhoto',
                    type: 'BANNER_MIRINDA',
                    count: 0
                },
                {
                    title: 'Banner Twister',
                    screen: 'POSMTakePhoto',
                    type: 'BANNER_TWISTER',
                    count: 0
                },
                {
                    title: 'Banner Revive',
                    screen: 'POSMTakePhoto',
                    type: 'BANNER_REVIVE',
                    count: 0
                },
                {
                    title: 'Banner Olong',
                    screen: 'POSMTakePhoto',
                    type: 'BANNER_OOLONG',
                    count: 0
                },
            ]
        };
    }

    componentDidMount = () => {
        BackHandler.addEventListener('hardwareBackPress', this.handleBackPress);
    }

    handleBackPress = () => {
        Alert.alert(
            STRINGS.MessageTitleAlert, 'Bạn chưa hoàn thành tiến trình công việc, bạn có chắc chắn thoát trang này, dữ liệu sẽ bị mất ?',
            [{
                text: STRINGS.MessageActionOK, onPress: () => {
                    this.props.navigation.navigate('Home');
                    return false;
                }
            },
            { text: STRINGS.MessageActionCancel, onPress: () => console.log('Cancel Pressed') }],
            { cancelable: false }
        );

        return true;
    }

    changeNumPOSM = (type) => {

        const { dataRes } = this.props;

        console.log('dataaaaaa', dataRes)

        if (dataRes != null) {
            var _posmParse = dataRes;
            var _data = this.state.data;

            for (let index = 0; index < _data.length; index++) {
                const element = _data[index];
                if(element.type === type)
                {
                    _data[index].count = 0;
                }
            }

            if (_posmParse.POSM != null && _posmParse.POSM.length != 0) {
                console.log("heeelo", _posmParse.POSM);

                for (let index = 0; index < _posmParse.POSM.length; index++) {
                    const element = _posmParse.POSM[index];

                    if (element.Code != type)
                        continue;
                    switch (element.Code) {
                        case 'DEFAULT':
                            _data[0].count += 1;
                            break;
                        case 'TRANH_PEPSI_AND_7UP':
                            _data[1].count += 1;
                            break;
                        case 'STICKER_7UP':
                            _data[2].count += 1;
                            break;
                        case 'STICKER_PEPSI':
                            _data[3].count += 1;
                            break;
                        case 'BANNER_PEPSI':
                            _data[4].count += 1;
                            break;
                        case 'BANNER_7UP_TET':
                            _data[5].count += 1;
                            break;
                        case 'BANNER_MIRINDA':
                            _data[6].count += 1;
                            break;
                        case 'BANNER_TWISTER':
                            _data[7].count += 1;
                            break;
                        case 'BANNER_REVIVE':
                            _data[8].count += 1;
                            break;
                        case 'BANNER_OOLONG':
                            _data[9].count += 1;
                            break;
                    }
                }
            }

            this.setState({ data: _data });
        }
    }

    handleGoto = (screen, type) => {
        this.props.navigation.navigate(screen, {
            type: type,
            refresh: this.changeNumPOSM.bind(this)
        });
    }

    handleDone = async () => {

        try {
            const _data = await AsyncStorage.getItem('DATA_SSC');
            const { dataRes } = this.props;

            if (dataRes != null) {
                let _posmParse = dataRes;

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
                        showsVerticalScrollIndicator={false}
                        style={{ padding: 0, width: CARD_WIDTH, marginBottom: 20 }}>
                        {data.map((item, index) => {
                            return (
                                <MainButton
                                    style={styles.button}
                                    styleTitle={item.count != 0 ? styles.buttonTitleRed : styles.buttonTitleWhite}
                                    title={item.title + ' (' + item.count.toString() + ')'}
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
    buttonTitleWhite: {
        color: 'white'
    },
    buttonTitleRed: {
        color: 'red'
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
        dataRes: state.POSMReducer.dataRes
    }
}

function dispatchToProps(dispatch) {
    return bindActionCreators({
    }, dispatch);
}

export default connect(mapStateToProps, dispatchToProps)(POSMOption);

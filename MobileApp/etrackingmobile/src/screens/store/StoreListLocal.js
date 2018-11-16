import React, { Component } from 'react';
import { View, StyleSheet, ScrollView, TouchableOpacity, Alert, AsyncStorage, Dimensions } from 'react-native';
import Spinner from 'react-native-loading-spinner-overlay';
import { Text, Item, Input } from 'native-base';
import { MainButton, MainHeader } from '../../components';
import { COLORS, FONTS, STRINGS } from '../../utils';
import PercentageCircle from 'react-native-percentage-circle';

import { bindActionCreators } from "redux";
import { connect } from "react-redux";
import {
    fetchPushDataToServer
} from '../../redux/actions/ActionPushDataToServer';

const { width, height } = Dimensions.get("window");
var RNFS = require('react-native-fs');

class StoreListLocal extends Component {
    constructor(props) {
        super(props);
        this.state = {
            data: [],
            isSubmit: false,
            percent: 0,
            plus: 0,
            dataPush: [],
            isDone: false
        };
    }

    componentWillMount = async () => {
        const _dataLocal = await AsyncStorage.getItem('DATA_SSC');
        if (_dataLocal != null) {
            let _dataParse = JSON.parse(_dataLocal);

            // var _backup = _dataParse;
            // _dataParse.push.apply(_dataParse, _backup);
            // _dataParse.push.apply(_dataParse, _backup);
            // _dataParse.push.apply(_dataParse, _backup);
            // _dataParse.push.apply(_dataParse, _backup);
            // _dataParse.push.apply(_dataParse, _backup);

            this.setState({ data: _dataParse });
        }
    }

    pushData = () => {
        const { dataPush } = this.state;

        try {
            if (dataPush.length != 0) {

                let element = dataPush[0];

                this.props.fetchPushDataToServer(element.Id, element.Code,
                    element.Date, '', element.Token, element.TrackSessionId, element.PosmNumber, element.Photo)
                    .then(() => setTimeout(() => {
                        this.bindData();
                    }, 100));
            }
            else {
                Alert.alert('Thông báo', 'Upload dữ liệu hoàn tất !!!');

                this.setState({ isDone: true });
            }
        } catch (error) {

        }
    }

    handleSubmit = () => {

        const { data } = this.state;

        if (data.length === 0) {
            Alert.alert('Thông báo', 'Không có dữ liệu !!!');
            return;
        }

        Alert.alert(
            STRINGS.MessageTitleAlert, 'Vui lòng không tắt màn hình này trong quá trình submit dữ liệu ?',
            [{
                text: STRINGS.MessageActionOK, onPress: () => {

                    let value = 0, _data = [];
                    const { data } = this.state;

                    data.forEach(item => {
                        value = value + item.POSM.length;
                        _data.push.apply(_data, item.POSM);
                    });

                    if (value !== 0) {
                        let _plus = 100 / value;
                        this.setState({ plus: _plus, dataPush: _data });
                    }

                    this.setState({ isSubmit: true });
                    this.pushData();
                }
            },
            { text: STRINGS.MessageActionCancel, onPress: () => console.log('Cancel Pressed') }],
            { cancelable: false }
        );
    }

    handleBack = () => {
        this.props.navigation.navigate('Home');
    }

    handleDone = () => {
        this.setState({ isSubmit: false, isDone: false, percent: 0, plus: 0 });
    }

    handleDeleteData = () => {

        const { data } = this.state;

        if (data.length === 0) {
            Alert.alert('Thông báo', 'Không có dữ liệu !!!');
            return;
        }

        Alert.alert(
            STRINGS.MessageTitleAlert, 'Bạn có chắc chắn muốn xoá hết dữ liệu ?',
            [{
                text: STRINGS.MessageActionOK, onPress: () => {

                    try {
                        AsyncStorage.removeItem('DATA_SSC');

                        // this.deleteDataLocal();

                        this.setState({ data: [] });

                        Alert.alert('Thông báo', 'Xoá dữ liệu thành công !!!');
                    } catch (error) {
                        Alert.alert('Thông báo', 'Xoá dữ liệu thất bại !!!');
                    }
                }
            },
            { text: STRINGS.MessageActionCancel, onPress: () => console.log('Cancel Pressed') }],
            { cancelable: false }
        );
    }

    deleteDataLocal = () => {
        const { data } = this.state;

        data.forEach(item => {
            Alert.alert(item.POSM.length+'');
            if (item.POSM.length !== 0) {
                item.POSM.forEach(element => {
                    // if (element.Photo.uri !== '') {
                        // Alert.alert(element.Photo.uri);
                        setTimeout(() => {
                            this.removeFile(element.Photo.uri);
                        }, 1000);
                        
                    // }
                });
            }
        });
    }

    removeFile = async (filepath) => {
        await RNFS.exists(filepath)
            .then((result) => {
                if (result) {
                    return RNFS.unlink(filepath)
                        .then(() => {
                            Alert.alert('FILE DELETED');
                        })
                        .catch((err) => {
                            Alert.alert('FILE not found');
                        });
                }
            })
            .catch((err) => {
                console.log(err.message);
            });
    }

    bindData = () => {
        const { PUSHdataRes, PUSHerror, PUSHerrorMessage } = this.props;

        const { plus, percent } = this.state;

        if (PUSHerror) {
            let _mess = PUSHerrorMessage + '';
            if (PUSHerrorMessage == 'TypeError: Network request failed')
                _mess = STRINGS.MessageNetworkError;

            Alert.alert(
                STRINGS.MessageTitleError, _mess,
                [{ text: STRINGS.MessageActionOK, onPress: () => console.log('OK Pressed') }], { cancelable: false }
            );

            return;
        }
        else {
            if (PUSHdataRes.HasError == true) {
                Alert.alert(
                    STRINGS.MessageTitleError, PUSHdataRes.Message + '',
                    [{ text: STRINGS.MessageActionOK, onPress: () => console.log('OK Pressed') }], { cancelable: false }
                );

                return;
            } else if (PUSHdataRes.HasError == false) {
                let _data = this.state.dataPush;
                _data = _data.splice(0, 1);

                this.setState({ percent: percent + plus });
                this.forceUpdate();

                this.pushData();
            }
        }
    }

    render() {

        const { data, isSubmit, percent, isDone } = this.state;

        return (
            <View
                style={styles.container}>
                <MainHeader
                    onPress={() => this.handleBack()}
                    hasLeft={true}
                    title={'Tải dữ liệu lên'} />
                {
                    isSubmit ?
                        <View
                            padder
                            style={styles.centerContainer}>
                            <PercentageCircle radius={70} percent={percent.toFixed(2)}
                                color={COLORS.BLUE_2E5665}
                                borderWidth={3}
                                textStyle={{ fontSize: 20, color: 'black' }}
                                innerColor={'#D6D2C7'}>
                            </PercentageCircle>
                            <Text style={styles.waring}>
                                {'Vui lòng không tắt màn hình này trong quá trình submit dữ liệu !!!'}
                            </Text>
                            {
                                isDone ?
                                    <MainButton
                                        style={styles.button}
                                        title={'Xong'}
                                        onPress={() => this.handleDone()} />
                                    : <View />
                            }
                        </View>
                        :
                        <View
                            padder
                            style={styles.subContainer}>

                            <ScrollView
                                horizontal={false}
                                showsHorizontalScrollIndicator={false}
                                contentContainerStyle={{
                                    marginBottom: 50,
                                }}
                                style={{ padding: 0 }}>

                                {data.map((item, index) => {
                                    return (
                                        <View style={styles.columnContainer}>
                                            <Text style={styles.title}>{item.Date}</Text>
                                            <Text style={styles.title}>Code: {item.Code}</Text>
                                            <Text style={styles.title}>Tên: {item.Name}</Text>
                                            <Text style={styles.title}>Hình ảnh: {item.POSM.length}</Text>
                                            <View style={styles.line} />
                                        </View>
                                    );
                                })}
                            </ScrollView>

                            <MainButton
                                style={styles.button}
                                title={'Submit'}
                                onPress={() => this.handleSubmit()} />

                            <MainButton
                                style={styles.button}
                                title={'Xoá hết dữ liệu'}
                                onPress={() => this.handleDeleteData()} />
                        </View>
                }

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
        padding: 20
    },
    centerContainer: {
        alignItems: 'center',
        justifyContent: 'center',
        flexDirection: 'column',
        paddingTop: 20,
        padding: 20,
        flex: 1
    },
    title: {
        fontFamily: FONTS.MAIN_FONT_REGULAR,
    },
    columnContainer: {
        flexDirection: 'column',
        justifyContent: 'center',
        margin: 5,
        width: width - 50
    },
    line: {
        height: 0.5,
        backgroundColor: COLORS.BLUE_2E5665,
        marginTop: 15,
        marginBottom: 15
    },
    waring: {
        color: 'red',
        textAlign: 'center',
        margin: 10,
        marginTop: 30
    }
});

function mapStateToProps(state) {
    return {
        PUSHdataRes: state.pushDataToServerReducer.dataRes,
        PUSHerror: state.pushDataToServerReducer.error,
        PUSHerrorMessage: state.pushDataToServerReducer.errorMessage,
    }
}

function dispatchToProps(dispatch) {
    return bindActionCreators({
        fetchPushDataToServer
    }, dispatch);
}

export default connect(mapStateToProps, dispatchToProps)(StoreListLocal);
import React, { Component } from 'react';
import { View, StyleSheet, ScrollView, TouchableOpacity, Alert, AsyncStorage, Dimensions, BackHandler, Platform } from 'react-native';
import { Text, CheckBox } from 'native-base';
import { MainButton, MainHeader } from '../../components';
import { COLORS, FONTS, STRINGS } from '../../utils';
import PercentageCircle from 'react-native-percentage-circle';
import RadioGroup from 'react-native-radio-buttons-group';

import { bindActionCreators } from "redux";
import { connect } from "react-redux";
import {
    fetchPushDataToServer,
    check
} from '../../redux/actions/ActionPushDataToServer';

const { width, height } = Dimensions.get("window");
var RNFS = require('react-native-fs');
import RNFetchBlob from 'rn-fetch-blob';

class StoreListLocal extends Component {
    constructor(props) {
        super(props);
        this.state = {
            isOK: true,
            data: [],
            dataDone: [],
            isSubmit: false,
            percent: 0,
            plus: 0,
            dataPush: [],
            dataDonePush: [],
            isDone: false,
            dataOption: [
                {
                    label: 'Chưa submit',
                    value: "0",
                },
                {
                    label: 'Đã submit',
                    value: "1",
                }
            ],
            isLoading: ''
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

        const _dataDone = await AsyncStorage.getItem('DATA_DONE_SSC');
        if (_dataDone != null) {
            let _dataParse = JSON.parse(_dataDone);

            // var _backup = _dataParse;
            // _dataParse.push.apply(_dataParse, _backup);
            // _dataParse.push.apply(_dataParse, _backup);
            // _dataParse.push.apply(_dataParse, _backup);
            // _dataParse.push.apply(_dataParse, _backup);
            // _dataParse.push.apply(_dataParse, _backup);

            this.setState({ dataDone: _dataParse });
        }
    }

    componentDidMount = () => {
        BackHandler.addEventListener('hardwareBackPress', this.handleBackPress);
    }

    componentWillUnmount = () =>{
        BackHandler.removeEventListener('hardwareBackPress', () => {});
    }

    pushData = async () => {
        const { dataPush } = this.state;

        try {
            if (dataPush.length != 0) {

                let element = dataPush[0];

                this.props.fetchPushDataToServer(element.Id, element.Code, element.Code2,
                    element.Date, '', element.Token, element.TrackSessionId, element.PosmNumber, element.Photo)
                    .then(() => setTimeout(() => {
                        this.bindData();
                    }, 100));
            }
            else {
                const { data, dataDone } = this.state;
                const _data = await AsyncStorage.getItem('DATA_DONE_SSC');

                if (_data != null) {
                    let dataParse = JSON.parse(_data);
                    dataParse.push.apply(dataParse, data);
                    AsyncStorage.setItem('DATA_DONE_SSC', JSON.stringify(dataParse));
                }
                else {
                    var _newData = [];
                    _newData.push.apply(_newData, data);
                    AsyncStorage.setItem('DATA_DONE_SSC', JSON.stringify(_newData));
                }
                let _d = dataDone;
                _d.push.apply(_d, data);
                this.setState({ dataDone: _d, data: [] });

                AsyncStorage.removeItem('DATA_SSC');

                this.setState({ isDone: true, isLoading: 'Tải lên hoàn tất !!!' });
                this.forceUpdate();
            }
        } catch (error) {

        }
    }

    pushDataDone = async () => {
        const { dataDonePush } = this.state;

        try {
            if (dataDonePush.length != 0) {

                let element = dataDonePush[0];

                this.props.fetchPushDataToServer(element.Id, element.Code, element.Code2,
                    element.Date, '', element.Token, element.TrackSessionId, element.PosmNumber, element.Photo)
                    .then(() => setTimeout(() => {
                        this.bindDataDone();
                    }, 100));
            }
            else {

                this.setState({ isDone: true, isLoading: 'Tải lên hoàn tất !!!' });
                this.forceUpdate();
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

                    this.setState({ isLoading: 'Đang kiểm tra dữ liệu !!!' });
                    this.forceUpdate();

                    for (let i = 0; i < data.length; i++) {
                        for (let index = 0; index < data[i].POSM.length; index++) {

                            if (Platform.OS == 'android') {
                                RNFS.stat(data[i].POSM[index].Photo.uri).then((StatResult) => {
                                    if (StatResult.originalFilepath != null && StatResult.originalFilepath != '') {
                                        RNFS.exists(StatResult.originalFilepath)
                                            .then((exist) => {
                                                if (exist) {
                                                    value = value + 1;
                                                    _data.push(data[i].POSM[index]);
                                                }
                                            })
                                            .catch(() => { })
                                    }

                                })
                            }
                            else {
                                RNFetchBlob.fs.exists(data[i].POSM[index].Photo.uri)
                                    .then((exist) => {
                                        if (exist) {
                                            value = value + 1;
                                            _data.push(data[i].POSM[index]);
                                        }
                                    })
                                    .catch(() => { })
                            }
                        }
                    }

                    setTimeout(() => {
                        if (value !== 0) {
                            let _plus = 100 / value;

                            this.setState({ plus: _plus, dataPush: _data });

                            this.setState({ isLoading: 'Đang tải dữ liệu lên !!!' });
                            this.pushData();
                        }
                        else {
                            this.setState({ isDone: true, isLoading: 'Không tìm thấy hình ảnh nào để tải lên !!!' });
                        }
                    }, 5000)

                    this.setState({ isSubmit: true });
                }
            },
            { text: STRINGS.MessageActionCancel, onPress: () => console.log('Cancel Pressed') }],
            { cancelable: false }
        );
    }

    handleSubmitDone = () => {

        const { dataDone } = this.state;

        if (dataDone.length === 0) {
            Alert.alert('Thông báo', 'Không có dữ liệu !!!');
            return;
        }

        let flag = false;
        dataDone.forEach(element => {
            if (element.IsSubmit) {
                flag = true;
            }
        });

        setTimeout(() => {
            if (flag === false) {
                Alert.alert('Thông báo', 'Chưa chọn dữ liệu !!!');
                return;
            }
            else {
                Alert.alert(
                    STRINGS.MessageTitleAlert, 'Bạn có chắc chắn muốn submit lại dữ liệu đã chọn ?',
                    [{
                        text: STRINGS.MessageActionOK, onPress: () => {

                            let value = 0, _data = [];
                            const { dataDone } = this.state;

                            this.setState({ isLoading: 'Đang kiểm tra dữ liệu !!!' });
                            this.forceUpdate();

                            for (let i = 0; i < dataDone.length; i++) {
                                if (dataDone[i].IsSubmit) {
                                    for (let index = 0; index < dataDone[i].POSM.length; index++) {

                                        if (Platform.OS === 'android') {
                                            RNFS.stat(dataDone[i].POSM[index].Photo.uri).then((StatResult) => {
                                                if (StatResult.originalFilepath != null && StatResult.originalFilepath != '') {
                                                    RNFS.exists(StatResult.originalFilepath)
                                                        .then((exist) => {
                                                            if (exist) {
                                                                value = value + 1;
                                                                _data.push(dataDone[i].POSM[index]);
                                                            }
                                                        })
                                                        .catch(() => { })
                                                }
                                            })
                                        }
                                        else {
                                            RNFetchBlob.fs.exists(dataDone[i].POSM[index].Photo.uri)
                                                .then((exist) => {
                                                    if (exist) {
                                                        value = value + 1;
                                                        _data.push(dataDone[i].POSM[index]);
                                                    }
                                                })
                                                .catch((error) => { })
                                        }
                                    }
                                }
                            }

                            setTimeout(() => {
                                if (value !== 0) {
                                    let _plus = 100 / value;
                                    this.setState({ plus: _plus, dataDonePush: _data });

                                    this.setState({ isLoading: 'Đang tải dữ liệu lên !!!' });
                                    this.pushDataDone();
                                }
                                else {
                                    this.setState({ isDone: true, isLoading: 'Không tìm thấy hình ảnh nào để tải lên !!!' });
                                }

                            }, 5000)

                            this.setState({ isSubmit: true });
                        }
                    },
                    { text: STRINGS.MessageActionCancel, onPress: () => console.log('Cancel Pressed') }],
                    { cancelable: false }
                );
            }
        }, 1000);
    }

    handleBack = () => {
        const { isSubmit, isDone } = this.state;

        if (!isSubmit) {
            this.props.navigation.navigate('Home');
        }
        else if (isSubmit && !isDone) {
            Alert.alert(
                STRINGS.MessageTitleAlert, 'Bạn đang upload dữ liệu, bạn có chắc chắn thoát trang này, dữ liệu có thể bị mất ?',
                [{
                    text: STRINGS.MessageActionOK, onPress: () => {
                        this.props.navigation.navigate('Home');
                    }
                },
                { text: STRINGS.MessageActionCancel, onPress: () => console.log('Cancel Pressed') }],
                { cancelable: false }
            );
        } else if (isSubmit && isDone) {
            this.handleDone();
        }
    }

    handleBackPress = () => {

        const { isSubmit, isDone } = this.state;

        if (!isSubmit) {
            this.props.navigation.navigate('Home');
            return false;
        }
        else if (isSubmit && !isDone) {
            Alert.alert(
                STRINGS.MessageTitleAlert, 'Bạn đang upload dữ liệu, bạn có chắc chắn thoát trang này, dữ liệu có thể bị mất ?',
                [{
                    text: STRINGS.MessageActionOK, onPress: () => {
                        this.props.navigation.navigate('Home');
                        return false;
                    }
                },
                { text: STRINGS.MessageActionCancel, onPress: () => console.log('Cancel Pressed') }],
                { cancelable: false }
            );
        } else if (isSubmit && isDone) {
            this.handleDone();
        }

        return true;
    }

    handleDone = () => {
        this.setState({ isSubmit: false, isDone: false, percent: 0, plus: 0 });
    }

    handleDeleteData = () => {

        const { dataDone } = this.state;

        if (dataDone.length === 0) {
            Alert.alert('Thông báo', 'Không có dữ liệu !!!');
            return;
        }

        Alert.alert(
            STRINGS.MessageTitleAlert, 'Bạn có chắc chắn muốn xoá hết dữ liệu ?',
            [{
                text: STRINGS.MessageActionOK, onPress: () => {

                    try {
                        AsyncStorage.removeItem('DATA_DONE_SSC');

                        // this.deleteDataLocal();

                        this.setState({ dataDone: [] });

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
        const { dataDone } = this.state;

        dataDone.forEach(item => {
            Alert.alert(item.POSM.length + '');
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

    bindDataDone = () => {
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
                let _data = this.state.dataDonePush;
                _data = _data.splice(0, 1);

                this.setState({ percent: percent + plus });
                this.forceUpdate();

                this.pushDataDone();
            }
        }
    }

    handleOptionChange(value) {
        this.setState({ isOK: false });
        value.forEach(element => {
            if (element.value === "0" && element.selected) {
                this.setState({ isOK: true });
                return;
            }
        });
    }

    checkPress = (item, index) => {
        var dataDone = this.state.dataDone;
        if (item.IsSubmit) {
            dataDone[index].IsSubmit = false;
        }
        else {
            dataDone[index].IsSubmit = true;
        }

        this.forceUpdate();
    }

    render() {

        const { data, dataDone, isSubmit, percent, isDone,
            isOK, isLoading } = this.state;

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
                                {isLoading}
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

                            <RadioGroup flexDirection='row' radioButtons={this.state.dataOption} onPress={value => this.handleOptionChange(value)} />

                            {
                                isOK ?
                                    <View>
                                        <ScrollView
                                            horizontal={false}
                                            showsHorizontalScrollIndicator={false}
                                            contentContainerStyle={{
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
                                    </View>
                                    :
                                    <View>
                                        <ScrollView
                                            horizontal={false}
                                            showsHorizontalScrollIndicator={false}
                                            contentContainerStyle={{
                                            }}
                                            style={{ padding: 0 }}>

                                            {dataDone.map((item, index) => {
                                                return (
                                                    <TouchableOpacity onPress={() => this.checkPress(item, index)}>
                                                        <View style={styles.columnContainer}>
                                                            <View style={styles.rowContainer}>
                                                                <CheckBox checked={item.IsSubmit} />
                                                                <View style={styles.columnContainer2}>
                                                                    <Text style={styles.title}>{item.Date}</Text>
                                                                    <Text style={styles.title}>Code: {item.Code}</Text>
                                                                    <Text style={styles.title}>Tên: {item.Name}</Text>
                                                                    <Text style={styles.title}>Hình ảnh: {item.POSM.length}</Text>
                                                                </View>
                                                            </View>
                                                            <View style={styles.line} />
                                                        </View>
                                                    </TouchableOpacity>
                                                );
                                            })}
                                        </ScrollView>

                                        <MainButton
                                            style={styles.button}
                                            title={'Submit'}
                                            onPress={() => this.handleSubmitDone()} />

                                        <MainButton
                                            style={styles.button}
                                            title={'Xoá hết dữ liệu'}
                                            onPress={() => this.handleDeleteData()} />
                                    </View>
                            }

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
        padding: 20,
        paddingBottom: 50
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
        // alignItems: 'center',
        margin: 5,
        width: width - 50
    },
    columnContainer2: {
        flexDirection: 'column',
        paddingLeft: 20
    },
    rowContainer: {
        flexDirection: 'row',
        // justifyContent: 'center',
        alignItems: "center"
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
    },
    button: {
        width: width - 40
    }
});

function mapStateToProps(state) {
    return {
        PUSHdataRes: state.pushDataToServerReducer.dataRes,
        PUSHerror: state.pushDataToServerReducer.error,
        PUSHerrorMessage: state.pushDataToServerReducer.errorMessage,
        isCheck: state.pushDataToServerReducer.isCheck
    }
}

function dispatchToProps(dispatch) {
    return bindActionCreators({
        fetchPushDataToServer,
        check
    }, dispatch);
}

export default connect(mapStateToProps, dispatchToProps)(StoreListLocal);
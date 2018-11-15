import React, { Component } from 'react';
import { View, StyleSheet, ScrollView, TouchableOpacity, Alert, AsyncStorage, Dimensions } from 'react-native';
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

class StoreListLocal extends Component {
    constructor(props) {
        super(props);
        this.state = {
            data: [],
            isSubmit: false,
            percent: 0,
            plus: 0
        };
    }

    componentWillMount = async () => {
        const _data = await AsyncStorage.getItem('DATA_SSC');
        if (_data != null) {
            let _dataParse = JSON.parse(_data);

            var _backup = _dataParse;

            // _dataParse.push.apply(_dataParse, _backup);
            // _dataParse.push.apply(_dataParse, _backup);
            // _dataParse.push.apply(_dataParse, _backup);
            // _dataParse.push.apply(_dataParse, _backup);
            // _dataParse.push.apply(_dataParse, _backup);

            this.setState({ data: _dataParse });

            let value = 0;

            data.forEach(item => {
                value = value + item.POSM.length;
            });

            if (value !== 0) {
                let _plus = 100 / value;
                this.setState({ plus: _plus });
            }

        }
    }

    handleSubmit = () => {
        this.setState({ isSubmit: true });

        const { data } = this.state;

        console.log('datanee', data);

        // const _data = await AsyncStorage.getItem('DATA_SSC');

        try {
            if (data != null) {

                if (data.length != 0) {

                    data.forEach(item => {
                        item.POSM.forEach(element => {
                            setTimeout(()=>{
                                this.props.fetchPushDataToServer(element.Id, element.Code,
                                    element.Date, element.MasterStoreId, element.Token, element.TrackSessionId, element.PosmNumber, element.Photo)
                                    .then(() => setTimeout(() => {
                                        this.bindData();
                                    }, 1000));
                            }, 1000);
                        });
                    });
                }

            }
        } catch (error) {
            console.log(error + '');
        }
    }

    handleBack = () => {
        this.props.navigation.navigate('Home');
    }

    bindData = () => {
        const { PUSHdataRes, PUSHerror, PUSHerrorMessage } = this.props;

        const {percent, plus }= this.state;

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
            } else if (PUSHdataRes.HasError == false) {
                this.setState({ percent: percent + plus });

                this.forceUpdate();
            }
        }
    }

    render() {

        const { data, isSubmit, percent } = this.state;

        return (
            <View
                style={styles.container}>
                <MainHeader
                    onPress={() => this.handleBack()}
                    hasLeft={true}
                    title={'Dữ liệu chưa submit'} />
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
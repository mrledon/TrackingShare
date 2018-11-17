import React, { Component } from 'react';
import { View, StyleSheet, Dimensions, Alert, ScrollView } from 'react-native';
import { Text } from 'native-base';
import { MainHeader } from '../../components';
import { COLORS, FONTS, STRINGS } from '../../utils';

import { bindActionCreators } from "redux";
import { connect } from "react-redux";
import {
    fetchGetDoneData
} from '../../redux/actions/ActionDoneData';

const { width, height } = Dimensions.get("window");

class StoreList extends Component {
    constructor(props) {
        super(props);
        this.state = {
            data: []
        };
    }

    componentWillMount = async () => {
        const { dataLogin } = this.props;

        await this.props.fetchGetDoneData(dataLogin.Data.Id)
            .then(() => setTimeout(() => {
                this.bindData()
            }, 100));
    }

    bindData() {
        const { dataRes, error, errorMessage } = this.props;

        if (error) {
            let _mess = errorMessage + '';
            if (errorMessage == 'TypeError: Network request failed')
                _mess = STRINGS.MessageNetworkError;

            Alert.alert(
                STRINGS.MessageTitleError, _mess,
                [{ text: STRINGS.MessageActionOK, onPress: () => console.log('OK Pressed') }], { cancelable: false }
            );
            return;
        }
        else {
            if (dataRes.HasError == true) {
                Alert.alert(
                    STRINGS.MessageTitleError, dataRes.Message + '',
                    [{ text: STRINGS.MessageActionOK, onPress: () => console.log('OK Pressed') }], { cancelable: false }
                );
            } else if (dataRes.HasError == false) {
                this.setState({ data: dataRes.Data });
            }
        }
    }

    handleBack = () => {
        this.props.navigation.navigate('Home');
    }

    render() {
        const { data } = this.state;
        return (
            <View
                style={styles.container}>
                <MainHeader
                    onPress={() => this.handleBack()}
                    hasLeft={true}
                    title={'Kết quả báo cáo'} />
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
                                    <Text style={styles.title}>{item.Store}</Text>
                                    <View style={styles.line} />
                                </View>
                            );
                        })}
                    </ScrollView>

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
        padding: 20
    },
    line: {
        height: 0.5,
        backgroundColor: COLORS.BLUE_2E5665,
        marginTop: 15,
        marginBottom: 15
    },
    columnContainer: {
        flexDirection: 'column',
        justifyContent: 'center',
        margin: 5,
        width: width - 50
    }
});

function mapStateToProps(state) {
    return {
        dataLogin: state.loginReducer.dataRes,

        isLoading: state.doneDataReducer.isLoading,
        dataRes: state.doneDataReducer.dataRes,
        error: state.doneDataReducer.error,
        errorMessage: state.doneDataReducer.errorMessage
    }
}

function dispatchToProps(dispatch) {
    return bindActionCreators({
        fetchGetDoneData
    }, dispatch);
}

export default connect(mapStateToProps, dispatchToProps)(StoreList);

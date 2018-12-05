import React, { Component } from 'react';
import { Platform, StyleSheet, View } from 'react-native';
import LinearGradient from 'react-native-linear-gradient';
import { Header, Title, Button, Left, Right, Body, Icon } from 'native-base';
import { COLORS, FONTS } from '../utils';

class MainHeader extends Component {
    render() {

        const { onPress, title, hasLeft } = this.props;

        return (
            <LinearGradient
                start={{ x: 0, y: 0.5 }}
                end={{ x: 1, y: 0.5 }}
                locations={[0.0, 0.7, 1.0]}
                colors={[COLORS.BLUE_2F6F7A, COLORS.BLUE_2E5665]}>
                <Header style={styles.header}>
                    <Left style={styles.left}>
                        {
                            hasLeft ?
                                <Button transparent>
                                    <Icon style={styles.icon} name='ios-arrow-back' onPress={onPress} />
                                </Button>
                                : <View />
                        }
                    </Left>
                    <Body style={styles.body}>
                        <Title style={styles.title}>{title.toLocaleUpperCase()}</Title>
                    </Body>
                    <Right style={styles.right} />
                </Header>
            </LinearGradient>
        );
    }
}

const styles = StyleSheet.create({
    header: {
        backgroundColor: 'transparent',
        flexDirection: 'row',
        justifyContent: 'center',
        alignItems: 'center'
    },
    body: {
        alignSelf: 'stretch',
        flex: 0.8,
        justifyContent: 'center',
        alignItems: 'center'
    },
    left: {
        paddingTop: Platform.OS === 'ios' ? 0 : 7,
        alignSelf: 'stretch', flex: 0.25,
    },
    icon: {
        fontSize: Platform.OS === 'ios' ? 30 : 35,
        color: COLORS.WHITE_FFFFFF,
        width: 70,
    },
    title: {
        fontFamily: FONTS.MAIN_FONT_REGULAR,
        color: COLORS.WHITE_FFFFFF
    },
    right: {
        alignSelf: 'stretch',
        flex: 0.25
    }
});

export { MainHeader };
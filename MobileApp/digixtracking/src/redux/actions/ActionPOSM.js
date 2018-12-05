import {
    SAVE_POSM
} from './types';

export const savePOSM = (posm) => {
    return {
        type: SAVE_POSM,
        posm
    };
}
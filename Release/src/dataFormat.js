/**
 * @description: 隐藏部分文字 显示为*号 如果只有两位,则后一个字替换成"*"
 * @param {*} str 传入的原始数据
 * @param {*} frontLen 前面需要保留几位
 * @param {*} endLen 后面需要保留几位
 * @return {*}
 */
export function hideStr(str, frontLen, endLen) {
    let newStr
    const len = str.length - frontLen - endLen
    if (str.length === 2) {
      newStr = `${str.substring(0, 1)}*`
    } else if (str.length > 2) {
      let char = ''
      for (let i = 0; i < len; i++) {
        char += '*'
      }
      newStr = str.substring(0, frontLen) + char + str.substring(str.length - endLen)
    } else {
      newStr = str
    }
    return newStr
  }

<template>
  <div class="p-4 max-w-md mx-auto">
    <!-- 使用者輸入提示詞的輸入框 -->
    <input
      v-model="userInput"
      type="text"
      placeholder="輸入提示詞..."
      class="w-full p-2 mb-4 border rounded"
      maxlength="100"
    />
    <!-- 點擊按鈕觸發圖像生成 -->
    <button @click="generateImage" class="w-full bg-blue-500 text-white p-2 rounded mb-4">
      生成圖像
    </button>
    <!-- 顯示目前狀態訊息 -->
    <p class="text-center text-blue-700 mb-4">{{ aiResponse }}</p>
    <!-- 顯示生成的圖像區域 -->
    <div class="w-full h-80 border flex justify-center items-center">
      <img v-if="imageSrc" :src="imageSrc" alt="AI 圖像" class="max-h-full max-w-full" />
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'

const userInput = ref('')
const aiResponse = ref('')
const imageSrc = ref('')
// 用於定期檢查圖像是否生成的 interval ID
let intervalId = null

// 點擊「生成圖像」按鈕後觸發的函式
const generateImage = async () => {
  if (!userInput.value.trim()) {
    aiResponse.value = '請輸入內容！'
    return
  }

  aiResponse.value = '正在生成圖像...'

  try {
    // 向後端發送圖像生成請求
    const response = await fetch('http://localhost:5093/api/greeting/generate-image', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({ Prompt: userInput.value }),
    })

    if (response.ok) {
      aiResponse.value = '圖像生成中，請稍候...'
      // 啟動圖像檢查機制
      startCheckingForImage()
    } else {
      aiResponse.value = '圖像生成請求失敗'
    }
  } catch (error) {
    aiResponse.value = '發生錯誤：' + error.message
  }
}

// 啟動每 5 秒檢查一次是否已有圖像
const startCheckingForImage = () => {
  if (intervalId) clearInterval(intervalId)
  intervalId = setInterval(checkForImage, 5000)
}

// 檢查後端是否已經生成圖像
const checkForImage = async () => {
  try {
    const response = await fetch('http://localhost:5093/api/greeting/check-image')

    if (response.ok) {
      const blob = await response.blob()
      // 建立圖片的暫時 URL 並顯示
      imageSrc.value = URL.createObjectURL(blob)
      aiResponse.value = '圖像生成成功！'
      clearInterval(intervalId)
    }
  } catch (error) {
    aiResponse.value = '發生錯誤：' + error.message
  }
}
</script>

<style scoped>
/* 調整輸入框和按鈕的字體大小 */
input {
  font-size: 16px;
}
button {
  font-size: 16px;
}
</style>

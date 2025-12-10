import express from 'express'
import redis from './redis.js'
import db from './db.js'

const app = express()
app.use(express.json())

app.get('/users', async (req, res) => {
  const cacheKey = 'users'

  const cached = await redis.get(cacheKey)
  if (cached) {
    return res.json({ source: 'redis', data: JSON.parse(cached) })
  }

  const response = await fetch('https://jsonplaceholder.typicode.com/photos')
  const data = await response.json()

  await redis.set(cacheKey, JSON.stringify(data), { EX: 300 })

  res.json({ source: 'mysql', data: data })
})

app.listen(4001, () => console.log('Server running on 3000'))

import { createClient } from 'redis'
const redis = createClient()

redis.connect()
  .then(() => console.log('Redis connected'))
  .catch(err => console.log(err))

export default redis

# Zod Kullanım Rehberi - Kapsamlı Eğitim

## 📚 İçindekiler
1. [Zod Nedir?](#zod-nedir)
2. [Kurulum](#kurulum)
3. [Temel Kullanım](#temel-kullanım)
4. [Veri Tipleri](#veri-tipleri)
5. [Validasyon Metodları](#validasyon-metodları)
6. [İleri Seviye Özellikler](#ileri-seviye-özellikler)
7. [React Hook Form ile Entegrasyon](#react-hook-form-ile-entegrasyon)
8. [Pratik Örnekler](#pratik-örnekler)
9. [Hata Yönetimi](#hata-yönetimi)
10. [Best Practices](#best-practices)

---

## Zod Nedir?

**Zod**, TypeScript ve JavaScript için güçlü bir **şema doğrulama (schema validation)** kütüphanesidir. Temel amacı:

- ✅ Veri doğrulama (validation)
- ✅ TypeScript tip çıkarımı (type inference)
- ✅ Runtime tip güvenliği
- ✅ Kullanıcı dostu hata mesajları

### Neden Zod Kullanmalıyız?

1. **Tip Güvenliği**: Şemalarınızdan otomatik TypeScript tipleri üretir
2. **Runtime Doğrulama**: Çalışma zamanında verilerinizi kontrol eder
3. **Kolay Kullanım**: Okunabilir ve anlaşılır API
4. **Form Entegrasyonu**: React Hook Form, Formik gibi kütüphanelerle mükemmel uyum
5. **Hata Mesajları**: Özelleştirilebilir ve anlaşılır hata mesajları

---

## Kurulum

```bash
npm install zod
# veya
yarn add zod
# veya
pnpm add zod
```

Projenizde zaten kurulu (v4.1.12). ✅

---

## Temel Kullanım

### Basit Bir Şema Oluşturma

```typescript
import { z } from 'zod'

// String şeması
const nameSchema = z.string()

// Doğrulama
const result = nameSchema.safeParse("Ahmet") // ✅ Başarılı
const result2 = nameSchema.safeParse(123)     // ❌ Hata
```

### Parse vs SafeParse

**`parse()`**: Hata durumunda exception fırlatır
```typescript
try {
  const name = nameSchema.parse(123) // ZodError fırlatır
} catch (error) {
  console.error(error.errors) // Hata detayları
}
```

**`safeParse()`**: Hata durumunda obje döner (önerilen)
```typescript
const result = nameSchema.safeParse(123)

if (result.success) {
  console.log(result.data) // Geçerli veri
} else {
  console.log(result.error.errors) // Hata detayları
}
```

---

## Veri Tipleri

### 1. String (Metin)

```typescript
z.string()                    // Herhangi bir string
z.string().min(1)            // En az 1 karakter
z.string().max(100)          // En fazla 100 karakter
z.string().length(10)        // Tam 10 karakter
z.string().email()           // E-posta formatı
z.string().url()             // URL formatı
z.string().uuid()            // UUID formatı
z.string().regex(/^[A-Z]+$/) // Regex ile özel format
```

**Projenizdeki Örnek:**
```8:19:Frontend/src/components/LoginForm.jsx
const loginSchema = z.object({
  email: z
    .string()
    .min(1, 'E-posta adresi gereklidir')
    .email('Geçerli bir e-posta adresi giriniz'),
  password: z
    .string()
    .min(1, 'Şifre gereklidir'),
  rememberMe: z.boolean().optional().default(false)
})
```

### 2. Number (Sayı)

```typescript
z.number()                    // Herhangi bir sayı
z.number().int()              // Tam sayı
z.number().positive()         // Pozitif sayı
z.number().negative()         // Negatif sayı
z.number().min(0)            // Minimum değer
z.number().max(100)          // Maksimum değer
z.number().multipleOf(5)      // 5'in katı
```

**Örnek:**
```typescript
const ageSchema = z.number().int().min(0).max(120)
const priceSchema = z.number().positive()
```

### 3. Boolean (Mantıksal)

```typescript
z.boolean()                   // true veya false
z.boolean().default(false)    // Varsayılan değer
```

**Projenizdeki Örnek:**
```19:19:Frontend/src/components/LoginForm.jsx
  rememberMe: z.boolean().optional().default(false)
```

### 4. Date (Tarih)

```typescript
z.date()                      // Date objesi
z.date().min(new Date())      // Gelecek tarih
z.date().max(new Date())      // Geçmiş tarih
```

**Örnek:**
```typescript
const birthDateSchema = z.date().max(new Date('2010-01-01'))
```

### 5. Array (Dizi)

```typescript
z.array(z.string())          // String dizisi
z.array(z.number()).min(1)   // En az 1 eleman
z.array(z.number()).max(10)  // En fazla 10 eleman
z.array(z.number()).length(5) // Tam 5 eleman
```

**Örnek:**
```typescript
const tagsSchema = z.array(z.string()).min(1, 'En az bir etiket seçmelisiniz')
```

### 6. Object (Nesne)

```typescript
z.object({
  name: z.string(),
  age: z.number()
})
```

**Projenizdeki Örnek:**
```8:19:Frontend/src/pages/Register.jsx
const registerSchema = z
  .object({
    firstName: z.string().min(1, 'Ad alanı zorunludur'),
    lastName: z.string().min(1, 'Soyad alanı zorunludur'),
    email: z.string().min(1, 'E-posta alanı zorunludur').email('Geçerli bir e-posta adresi giriniz'),
    password: z.string().min(6, 'Şifre en az 6 karakter olmalıdır'),
    confirmPassword: z.string().min(6, 'Şifre tekrar alanı zorunludur')
  })
  .refine(data => data.password === data.confirmPassword, {
    message: 'Şifreler eşleşmiyor',
    path: ['confirmPassword']
  })
```

### 7. Optional ve Nullable

```typescript
z.string().optional()        // undefined olabilir
z.string().nullable()        // null olabilir
z.string().nullish()         // null veya undefined olabilir
z.string().default("varsayılan") // Varsayılan değer
```

**Örnek:**
```typescript
const userSchema = z.object({
  name: z.string(),
  email: z.string().email(),
  phone: z.string().optional(),      // Opsiyonel
  notes: z.string().nullable(),       // null olabilir
  status: z.string().default("active") // Varsayılan değer
})
```

### 8. Union (Birleşim)

```typescript
z.union([z.string(), z.number()])  // String veya number
z.string().or(z.number())          // Aynı şey (kısa yazım)
```

**Örnek:**
```typescript
const idSchema = z.union([z.string(), z.number()])
// veya
const idSchema = z.string().or(z.number())
```

### 9. Enum (Sabit Değerler)

```typescript
z.enum(['admin', 'user', 'guest'])  // Sadece bu değerler
```

**Örnek:**
```typescript
const roleSchema = z.enum(['admin', 'user', 'moderator'])
```

### 10. Literal (Tam Eşleşme)

```typescript
z.literal('success')          // Sadece 'success' değeri
z.literal(42)                 // Sadece 42 değeri
```

---

## Validasyon Metodları

### Chaining (Zincirleme)

Birden fazla validasyonu birleştirebilirsiniz:

```typescript
const emailSchema = z
  .string()
  .min(1, 'E-posta gereklidir')
  .email('Geçerli bir e-posta giriniz')
  .toLowerCase()              // Otomatik küçük harfe çevir
```

### Custom Validation (Özel Doğrulama)

**`.refine()`**: Karmaşık doğrulamalar için

```typescript
const passwordSchema = z
  .string()
  .min(8)
  .refine(
    (password) => /[A-Z]/.test(password),
    { message: 'En az bir büyük harf içermelidir' }
  )
  .refine(
    (password) => /[0-9]/.test(password),
    { message: 'En az bir rakam içermelidir' }
  )
```

**Projenizdeki Örnek:**
```16:19:Frontend/src/pages/Register.jsx
  .refine(data => data.password === data.confirmPassword, {
    message: 'Şifreler eşleşmiyor',
    path: ['confirmPassword']
  })
```

**`.superRefine()`**: Daha gelişmiş hata kontrolü

```typescript
const schema = z.string().superRefine((val, ctx) => {
  if (val.length < 5) {
    ctx.addIssue({
      code: z.ZodIssueCode.custom,
      message: "Çok kısa"
    })
  }
})
```

### Transform (Dönüştürme)

Veriyi doğrulama sırasında dönüştürebilirsiniz:

```typescript
const numberStringSchema = z.string().transform((val) => Number(val))
// "123" → 123

const trimSchema = z.string().transform((val) => val.trim())
// "  ahmet  " → "ahmet"
```

### Preprocess (Ön İşleme)

Veriyi doğrulamadan önce işleyebilirsiniz:

```typescript
const numberSchema = z.preprocess(
  (val) => (typeof val === 'string' ? Number(val) : val),
  z.number()
)
```

---

## İleri Seviye Özellikler

### 1. Nested Objects (İç İçe Nesneler)

```typescript
const addressSchema = z.object({
  street: z.string(),
  city: z.string(),
  zipCode: z.string()
})

const userSchema = z.object({
  name: z.string(),
  address: addressSchema  // İç içe şema
})

// veya inline
const userSchema = z.object({
  name: z.string(),
  address: z.object({
    street: z.string(),
    city: z.string()
  })
})
```

### 2. Arrays of Objects (Nesne Dizileri)

```typescript
const userSchema = z.object({
  name: z.string(),
  hobbies: z.array(z.object({
    name: z.string(),
    level: z.enum(['beginner', 'intermediate', 'advanced'])
  }))
})
```

### 3. Partial (Kısmi Şema)

Tüm alanları opsiyonel yapar:

```typescript
const fullUserSchema = z.object({
  name: z.string(),
  email: z.string().email(),
  age: z.number()
})

const partialUserSchema = fullUserSchema.partial()
// { name?: string, email?: string, age?: number }
```

### 4. Pick ve Omit (Seçme ve Çıkarma)

```typescript
const userSchema = z.object({
  name: z.string(),
  email: z.string(),
  password: z.string(),
  age: z.number()
})

// Sadece belirli alanları al
const publicUserSchema = userSchema.pick({ name: true, email: true })

// Belirli alanları çıkar
const safeUserSchema = userSchema.omit({ password: true })
```

### 5. Extend (Genişletme)

```typescript
const baseSchema = z.object({
  name: z.string(),
  email: z.string()
})

const extendedSchema = baseSchema.extend({
  age: z.number(),
  phone: z.string().optional()
})
```

### 6. Merge (Birleştirme)

```typescript
const schema1 = z.object({ name: z.string() })
const schema2 = z.object({ age: z.number() })

const mergedSchema = schema1.merge(schema2)
// { name: string, age: number }
```

### 7. Discriminated Union (Ayrıştırılmış Birleşim)

```typescript
const dogSchema = z.object({
  type: z.literal('dog'),
  bark: z.boolean()
})

const catSchema = z.object({
  type: z.literal('cat'),
  meow: z.boolean()
})

const petSchema = z.discriminatedUnion('type', [dogSchema, catSchema])
```

---

## React Hook Form ile Entegrasyon

Zod, React Hook Form ile mükemmel çalışır. Projenizde de kullanılıyor! ✅

### Kurulum

```bash
npm install @hookform/resolvers zod react-hook-form
```

### Kullanım

**1. Şema Oluşturma:**
```typescript
import { z } from 'zod'

const formSchema = z.object({
  email: z.string().email('Geçerli bir e-posta giriniz'),
  password: z.string().min(6, 'Şifre en az 6 karakter olmalıdır')
})
```

**2. React Hook Form ile Entegrasyon:**
```typescript
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'

function MyForm() {
  const {
    register,
    handleSubmit,
    formState: { errors }
  } = useForm({
    resolver: zodResolver(formSchema),
    defaultValues: {
      email: '',
      password: ''
    }
  })

  const onSubmit = (data) => {
    // data otomatik olarak doğrulanmış ve tip güvenli
    console.log(data)
  }

  return (
    <form onSubmit={handleSubmit(onSubmit)}>
      <input {...register('email')} />
      {errors.email && <span>{errors.email.message}</span>}
      
      <input type="password" {...register('password')} />
      {errors.password && <span>{errors.password.message}</span>}
      
      <button type="submit">Gönder</button>
    </form>
  )
}
```

**Projenizdeki Gerçek Örnek:**
```27:41:Frontend/src/pages/Register.jsx
  const {
    register,
    handleSubmit,
    reset,
    formState: { errors }
  } = useForm({
    resolver: zodResolver(registerSchema),
    defaultValues: {
      firstName: '',
      lastName: '',
      email: '',
      password: '',
      confirmPassword: ''
    }
  })
```

### TypeScript ile Tip Çıkarımı

```typescript
import { z } from 'zod'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'

const formSchema = z.object({
  email: z.string().email(),
  age: z.number().min(18)
})

// Tip otomatik çıkarılır
type FormData = z.infer<typeof formSchema>

function MyForm() {
  const { register, handleSubmit } = useForm<FormData>({
    resolver: zodResolver(formSchema)
  })
  
  // ...
}
```

---

## Pratik Örnekler

### 1. Kullanıcı Kayıt Formu

```typescript
const registerSchema = z.object({
  firstName: z.string().min(2, 'Ad en az 2 karakter olmalıdır'),
  lastName: z.string().min(2, 'Soyad en az 2 karakter olmalıdır'),
  email: z.string().email('Geçerli bir e-posta adresi giriniz'),
  password: z
    .string()
    .min(8, 'Şifre en az 8 karakter olmalıdır')
    .regex(/[A-Z]/, 'En az bir büyük harf içermelidir')
    .regex(/[0-9]/, 'En az bir rakam içermelidir'),
  confirmPassword: z.string(),
  age: z.number().int().min(18, '18 yaşından büyük olmalısınız'),
  terms: z.boolean().refine(val => val === true, {
    message: 'Şartları kabul etmelisiniz'
  })
}).refine(data => data.password === data.confirmPassword, {
  message: 'Şifreler eşleşmiyor',
  path: ['confirmPassword']
})
```

### 2. API Request/Response Validasyonu

```typescript
// API'den gelen veriyi doğrula
const userResponseSchema = z.object({
  id: z.number(),
  name: z.string(),
  email: z.string().email(),
  createdAt: z.string().datetime()
})

async function fetchUser(id: number) {
  const response = await fetch(`/api/users/${id}`)
  const data = await response.json()
  
  // Veriyi doğrula
  const validatedData = userResponseSchema.parse(data)
  return validatedData // Tip güvenli!
}
```

### 3. Form State Yönetimi

```typescript
const productSchema = z.object({
  name: z.string().min(1),
  price: z.number().positive(),
  category: z.enum(['electronics', 'clothing', 'food']),
  tags: z.array(z.string()).min(1),
  description: z.string().optional()
})

// Form submit edilmeden önce doğrula
function ProductForm() {
  const [formData, setFormData] = useState({})
  
  const handleSubmit = (e) => {
    e.preventDefault()
    
    const result = productSchema.safeParse(formData)
    
    if (!result.success) {
      // Hataları göster
      result.error.errors.forEach(err => {
        console.error(`${err.path.join('.')}: ${err.message}`)
      })
      return
    }
    
    // Doğrulanmış veriyi gönder
    submitProduct(result.data)
  }
}
```

### 4. Environment Variables (Ortam Değişkenleri)

```typescript
const envSchema = z.object({
  DATABASE_URL: z.string().url(),
  API_KEY: z.string().min(1),
  PORT: z.string().transform(Number).pipe(z.number().int().positive()),
  NODE_ENV: z.enum(['development', 'production', 'test'])
})

const env = envSchema.parse(process.env)
// Artık env değişkenleri tip güvenli!
```

### 5. Dinamik Form Validasyonu

```typescript
const createDynamicSchema = (minAge: number) => {
  return z.object({
    name: z.string(),
    age: z.number().min(minAge, `Yaş en az ${minAge} olmalıdır`),
    email: z.string().email()
  })
}

// Kullanım
const adultSchema = createDynamicSchema(18)
const seniorSchema = createDynamicSchema(65)
```

---

## Hata Yönetimi

### Hata Formatı

```typescript
const result = schema.safeParse(invalidData)

if (!result.success) {
  result.error.errors.forEach((error) => {
    console.log({
      path: error.path,        // ['email'] - Hatanın yolu
      message: error.message,   // "Geçerli bir e-posta giriniz"
      code: error.code         // "invalid_string"
    })
  })
}
```

### Özel Hata Mesajları

```typescript
const schema = z.object({
  email: z.string({
    required_error: "E-posta gereklidir",
    invalid_type_error: "E-posta bir string olmalıdır"
  }).email("Geçerli bir e-posta adresi giriniz"),
  
  age: z.number({
    required_error: "Yaş gereklidir",
    invalid_type_error: "Yaş bir sayı olmalıdır"
  }).min(18, "18 yaşından büyük olmalısınız")
})
```

### Hata Mesajlarını Özelleştirme

```typescript
import { z } from 'zod'

// Global hata mesajları
z.setErrorMap((issue, ctx) => {
  if (issue.code === z.ZodIssueCode.invalid_type) {
    if (issue.expected === 'string') {
      return { message: 'Bu alan bir metin olmalıdır' }
    }
  }
  return { message: ctx.defaultError }
})

// Şema bazlı özelleştirme
const schema = z.string({
  errorMap: (issue, ctx) => {
    if (issue.code === 'too_small') {
      return { message: 'Çok kısa!' }
    }
    return { message: ctx.defaultError }
  }
})
```

### Hata Formatlama

```typescript
function formatZodErrors(error: z.ZodError) {
  return error.errors.map(err => ({
    field: err.path.join('.'),
    message: err.message
  }))
}

// Kullanım
const result = schema.safeParse(data)
if (!result.success) {
  const formattedErrors = formatZodErrors(result.error)
  // [{ field: 'email', message: 'Geçerli bir e-posta giriniz' }]
}
```

---

## Best Practices (En İyi Uygulamalar)

### 1. Şemaları Ayrı Dosyalarda Tutun

```typescript
// schemas/userSchema.ts
import { z } from 'zod'

export const userSchema = z.object({
  name: z.string(),
  email: z.string().email()
})

export type User = z.infer<typeof userSchema>
```

### 2. Şemaları Yeniden Kullanın

```typescript
// schemas/commonSchemas.ts
export const emailSchema = z.string().email('Geçerli bir e-posta giriniz')
export const passwordSchema = z.string().min(8, 'Şifre en az 8 karakter olmalıdır')

// schemas/userSchema.ts
import { emailSchema, passwordSchema } from './commonSchemas'

export const loginSchema = z.object({
  email: emailSchema,
  password: passwordSchema
})
```

### 3. TypeScript Tiplerini Çıkarın

```typescript
const userSchema = z.object({
  name: z.string(),
  age: z.number()
})

// Otomatik tip çıkarımı
type User = z.infer<typeof userSchema>
// { name: string, age: number }

// Input/Output ayrımı
type UserInput = z.input<typeof userSchema>   // Transform öncesi
type UserOutput = z.output<typeof userSchema> // Transform sonrası
```

### 4. safeParse Kullanın

```typescript
// ❌ Kötü - Exception fırlatır
try {
  const data = schema.parse(unknownData)
} catch (error) {
  // Hata yönetimi
}

// ✅ İyi - Güvenli parse
const result = schema.safeParse(unknownData)
if (result.success) {
  // Başarılı
} else {
  // Hata yönetimi
}
```

### 5. Transform ile Veri Temizleme

```typescript
const cleanStringSchema = z.string().transform(val => val.trim().toLowerCase())

const userSchema = z.object({
  email: z.string().email().transform(val => val.toLowerCase()),
  name: z.string().transform(val => val.trim())
})
```

### 6. Partial ile Update İşlemleri

```typescript
const createUserSchema = z.object({
  name: z.string(),
  email: z.string().email(),
  age: z.number()
})

// Update için kısmi şema
const updateUserSchema = createUserSchema.partial()

// Kullanım
function updateUser(id: number, data: z.infer<typeof updateUserSchema>) {
  // Sadece gönderilen alanlar güncellenir
}
```

### 7. Async Validation (Zaman Uyumsuz Doğrulama)

```typescript
const uniqueEmailSchema = z.string().email().refine(
  async (email) => {
    const exists = await checkEmailExists(email)
    return !exists
  },
  { message: 'Bu e-posta zaten kullanılıyor' }
)
```

### 8. Şema Versiyonlama

```typescript
// v1
const userSchemaV1 = z.object({
  name: z.string()
})

// v2
const userSchemaV2 = z.object({
  firstName: z.string(),
  lastName: z.string()
})

// Migration
function migrateV1ToV2(v1Data: z.infer<typeof userSchemaV1>) {
  return {
    firstName: v1Data.name.split(' ')[0],
    lastName: v1Data.name.split(' ').slice(1).join(' ')
  }
}
```

---

## Sık Kullanılan Şema Örnekleri

### Türk Telefon Numarası

```typescript
const turkishPhoneSchema = z
  .string()
  .regex(/^(\+90|0)?[5][0-9]{9}$/, 'Geçerli bir Türk telefon numarası giriniz')
  .transform(val => val.replace(/^(\+90|0)/, '')) // +90 veya 0'ı kaldır
```

### TC Kimlik No

```typescript
const tcKimlikSchema = z
  .string()
  .length(11, 'TC Kimlik No 11 haneli olmalıdır')
  .regex(/^[0-9]+$/, 'TC Kimlik No sadece rakam içermelidir')
```

### Türk Lirası Formatı

```typescript
const priceSchema = z
  .string()
  .regex(/^\d+([.,]\d{2})?$/, 'Geçerli bir fiyat giriniz')
  .transform(val => parseFloat(val.replace(',', '.')))
  .pipe(z.number().positive())
```

### Tarih Aralığı

```typescript
const dateRangeSchema = z.object({
  startDate: z.string().datetime(),
  endDate: z.string().datetime()
}).refine(
  data => new Date(data.endDate) > new Date(data.startDate),
  {
    message: 'Bitiş tarihi başlangıç tarihinden sonra olmalıdır',
    path: ['endDate']
  }
)
```

---

## Özet: Zod'un Avantajları

1. ✅ **Tip Güvenliği**: TypeScript ile mükemmel uyum
2. ✅ **Runtime Doğrulama**: Çalışma zamanında veri kontrolü
3. ✅ **Okunabilirlik**: Temiz ve anlaşılır API
4. ✅ **Hata Mesajları**: Özelleştirilebilir ve anlaşılır
5. ✅ **Form Entegrasyonu**: React Hook Form ile kolay kullanım
6. ✅ **Performans**: Hızlı ve optimize edilmiş
7. ✅ **Genişletilebilirlik**: Özel validasyonlar eklenebilir
8. ✅ **Topluluk Desteği**: Aktif ve büyük bir topluluk

---

## Kaynaklar

- [Zod Resmi Dokümantasyon](https://zod.dev/)
- [Zod GitHub](https://github.com/colinhacks/zod)
- [React Hook Form + Zod](https://react-hook-form.com/get-started#SchemaValidation)

---

## Sonuç

Zod, modern JavaScript/TypeScript projelerinde veri doğrulama için güçlü ve esnek bir araçtır. Projenizde zaten kullanılıyor ve React Hook Form ile entegre edilmiş durumda. Bu rehberdeki örnekleri kullanarak daha karmaşık validasyonlar oluşturabilirsiniz.

**İyi kodlamalar! 🚀**


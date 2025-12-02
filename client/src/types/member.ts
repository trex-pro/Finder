export type Member =  {
  id: string
  dob: string
  imageUrl?: string
  userName: string
  created: string
  lastActive: string
  gender: string
  description?: string
  city: string
  country: string
}

export type Photo = {
  id: number
  url: string
  publicId?: any
  memberId: string
}

export type EditableMember = {
  userName: string;
  description?: string
  city: string
  country: string
}
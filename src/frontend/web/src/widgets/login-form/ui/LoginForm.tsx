import { cn } from "@/shared/lib/utils.ts"
import { Button } from "@/shared/ui/button.tsx"
import {
  Card,
  CardContent,
  CardHeader,
  CardTitle,
} from "@/shared/ui/card.tsx"
import {
  Field,
  FieldDescription,
  FieldGroup,
  FieldLabel,
} from "@/shared/ui/field.tsx"
import { Input } from "@/shared/ui/input.tsx"
import { Form } from "react-router-dom"

interface LoginFormProps extends React.ComponentProps<"div"> {
  error?: string;
  isSubmitting?: boolean;
}

export function LoginForm({
  error,
  isSubmitting = false,
  className,
  ...props
}: LoginFormProps) {
  return (
    <div className={cn("flex flex-col gap-6", className)} {...props}>
      <Card>
        <CardHeader>
          <CardTitle>Login to your account</CardTitle>
        </CardHeader>
        <CardContent>
          <Form method="POST">
            <FieldGroup>
              <Field>
                <FieldLabel htmlFor="username">Username/Email</FieldLabel>
                <Input
                  name="identifier"
                  id="username"
                  type="text"
                  placeholder="username"
                  required
                />
              </Field>
              <Field>
                <div className="flex items-center">
                  <FieldLabel htmlFor="password">Password</FieldLabel>
                </div>
                <Input name="password" id="password" type="password" required />
              </Field>
              <Field>
                <Button type="submit" disabled={isSubmitting}>{isSubmitting ? "Logging in..." : "Login"}</Button>
                {error && (
                    <FieldDescription className="text-center text-red-500 font-medium mt-2">
                      {error}
                    </FieldDescription>
                )}
              </Field>
            </FieldGroup>
          </Form>
        </CardContent>
      </Card>
    </div>
  )
}
